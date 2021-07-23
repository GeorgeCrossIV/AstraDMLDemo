using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Models;
using Newtonsoft.Json.Converters;

namespace AstraDMLDemo.Repositories
{
    public class DseRestProductRepository : IProductRepository
    {
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initialize Rest Repository and construct class
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public DseRestProductRepository(IWebHostEnvironment host, IConfiguration config)
        {
            _host = host;
            _config = config;
        }

        /// <summary>
        /// Get Rest repository definition
        /// </summary>
        public RepositoryDefinition RepositoryDefinition
        {
            get
            {
                return AstraDMLDemo.Program._repositoryDefinitions
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.DseRest.ToString());
            }
        }

        /// <summary>
        /// Checks to validate connectivity to the REST API endpoint
        /// </summary>
        /// <returns></returns>
        public bool IsRepositoryAvailable()
        {
            try
            {
                //Set API URL
                var url = string.Format($"http://{RepositoryDefinition.Address}:8082/v1/keyspaces" +
                    $"/{RepositoryDefinition.Keyspace}/tables");

                //Execute API Get 
                IRestResponse response = Utils.Api.Get(url, GetToken());

                //Get search results
                string[] tables = JsonConvert.DeserializeObject<string[]>(response.Content);

                if (tables.FirstOrDefault(t => t == "products") != null)
                {
                    return true; // round a table named products
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get authentication token
        /// </summary>
        /// <returns></returns>
        private string GetToken()
        {
            if (RepositoryDefinition.Token == null)
            {
                //Set API URL
                var tokenUrl = string.Format($"http://{RepositoryDefinition.Address}:8081/v1/auth");

                // get the token
                IRestResponse tokenResponse = Utils.Api.GetToken(tokenUrl,
                    RepositoryDefinition.UserClientId, RepositoryDefinition.PasswordSecret);

                // Obtain token from the results
                JObject tokenObject = JObject.Parse(tokenResponse.Content);
                JToken tokenToken = tokenObject.SelectToken("authToken");

                string token = tokenToken.ToString();
                return token;
            }
            else
            {
                return RepositoryDefinition.Token;
            }
        }

        /* Implmentation of REST API CRUD Operations */

        /// <summary>
        /// [REST] Creates new Product row
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Add(Product product)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082/v2/keyspaces" +
                $"/{RepositoryDefinition.Keyspace}/{RepositoryDefinition.TableName}");


            //Prepare request body
            product.GetCorrectedTimestamp();

            // Execute API Post 
            IRestResponse response = Utils.Api.Post(url, GetToken(), JsonConvert.SerializeObject(product));

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The ADD Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The ADD Rest API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// [REST] Read function to get Product with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProduct(Guid id)
        {
            ////Set API URL
            var url = string.Format($"http://{this.RepositoryDefinition.Address}:8082/" +
                $"v1/keyspaces/{RepositoryDefinition.Keyspace}" +
                $"/tables/{RepositoryDefinition.TableName}/rows/{id.ToString()}");

            // Execute API Get 
            IRestResponse response = Utils.Api.Get(url, GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GET Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
            }
            else
            {
                Console.WriteLine(String.Format("The Get Rest API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }

            //Parse response
            JToken result = JToken.Parse(response.Content);
            JToken data = result.SelectToken("rows");

            //Deserialize JSON to Product
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(data.ToString());
            Product product = products.First();

            return product;
        }

        /// <summary>
        /// [REST] Read function to get all rows in the Products table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProduct()
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082/v1/keyspaces" +
                $"/{RepositoryDefinition.Keyspace}/tables/{this.RepositoryDefinition.TableName}/rows");

            // Execute API Get 
            IRestResponse response = Utils.Api.Get(url, GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GETROWS Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
            }
            else
            {
                Console.WriteLine(String.Format("The GETROWS Rest API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }

            // Format search results
            JObject jObject = JObject.Parse(response.Content);
            JToken result = JToken.Parse(response.Content);
            JToken data = result.SelectToken("rows");

            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(data.ToString());

            return products;
        }

        /// <summary>
        /// [REST] Update function to update a partial Product row with the given values
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/keyspaces/{RepositoryDefinition.Keyspace}/{RepositoryDefinition.TableName}/{product.id.ToString()}");

            product.GetCorrectedTimestamp();

            JObject jo = JObject.Parse(JsonConvert.SerializeObject(product));
            jo.Property("id").Remove();         //Remove primary key
            jo.Property("field1").Remove();     //Remove "extra" columns fields 1-10
            jo.Property("field2").Remove();
            jo.Property("field3").Remove();
            jo.Property("field4").Remove();
            jo.Property("field5").Remove();
            jo.Property("field6").Remove();
            jo.Property("field7").Remove();
            jo.Property("field8").Remove();
            jo.Property("field9").Remove();
            jo.Property("field10").Remove();

            //Execute API Patch
            var response = Utils.Api.Patch(url, this.GetToken(), jo.ToString());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The UPDATE Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The UPDATE Rest API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// [REST] Delete function to delete row from Products table
        /// </summary>
        /// <param name="id"></param>
        void IProductRepository.Delete(Guid id)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082/v2/keyspaces" +
                $"/{RepositoryDefinition.Keyspace}/{RepositoryDefinition.TableName}/{id}");

            // Execute API Delete 
            IRestResponse response = Utils.Api.Delete(url, this.GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The DELETE Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return;
            }
            else
            {
                Console.WriteLine(String.Format("The DELETE Rest API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception(response.StatusCode.ToString());

            }
        }
    }
}
