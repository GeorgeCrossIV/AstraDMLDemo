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


namespace AstraDMLDemo.Repositories
{
    public class DseDocumentProductRepository : IProductRepository
    {
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initialize Document API Repository and construct class
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public DseDocumentProductRepository(IWebHostEnvironment host, IConfiguration config)
        {
            _host = host;
            _config = config;
        }

        /// <summary>
        /// Get Document repository definition
        /// </summary>
        public RepositoryDefinition RepositoryDefinition
        {
            get
            {
                return AstraDMLDemo.Program._repositoryDefinitions
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.DseDocument.ToString());
            }
        }

        /// <summary>
        /// Checks to validate connectivity to the Document API endpoint
        /// </summary>
        /// <returns></returns>
        public bool IsRepositoryAvailable()
        {
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces/{RepositoryDefinition.Keyspace}/collections");

            // Execute API Post 
            var response = Utils.Api.Get(url, GetToken());

            // Parse results
            JObject jObject = JObject.Parse(response.Content);
            JToken result = JToken.Parse(response.Content);

            JToken table = result.SelectToken("$.data[?(@.name == '" + this.RepositoryDefinition.TableName + "')]");

            if (table != null)
            {
                return true;
            }
            else
            {
                return false;
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

        /* Implmentation of Document API CRUD Operations */

        /// <summary>
        /// [DOCUMENT] Creates new Product document
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Add(Product product)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces/{RepositoryDefinition.Keyspace}" +
                $"/collections/products_collection/{product.id.ToString()}");

            // Execute API Post 
            var response = Utils.Api.Put(url, GetToken(), JsonConvert.SerializeObject(product));

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The ADD Rest API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The ADD Rest API command was sent but failed to be processed correctly by the server with status code: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// [DOCUMENT] Read function to get Product document with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProduct(Guid id)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces/{RepositoryDefinition.Keyspace}" +
                $"/collections/{RepositoryDefinition.TableName}/{id}");

            // Execute API Post 
            var response = Utils.Api.Get(url, GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GET Document API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
            }
            else
            {
                Console.WriteLine(String.Format("The Get Document API command was sent but failed to be processed correctly by the server with status code: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }

            // Parse results
            JObject jObject = JObject.Parse(response.Content);
            JToken result = JToken.Parse(response.Content);
            JToken data = result.SelectToken("data");

            Product product = JsonConvert.DeserializeObject<Product>(data.ToString());

            return product;
        }

        /// <summary>
        /// [DOCUMENT] Read function to get all Documents in the Products collection
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProduct()
        {
            Product product;
            List<Product> products = new List<Product>();

            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces/{RepositoryDefinition.Keyspace}" +
                $"/collections/products_collection?page-size=20");

            // Execute API Post 
            var response = Utils.Api.Get(url, GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GETROWS Document API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
            }
            else
            {
                Console.WriteLine(String.Format("The GETROWS Document API command was sent but failed to be processed correctly by the server with status code: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                throw new Exception(response.StatusCode.ToString());

            }

            // Parse results
            JObject jObject = JObject.Parse(response.Content);
            JToken result = JToken.Parse(response.Content);
            JToken data = result.SelectToken("data");
            foreach (var x in data.Children())
            {
                var dataJson = x.ToString().Split(':', 2)[1];
                product = JsonConvert.DeserializeObject<Product>(dataJson);
                products.Add(product);
            }

            return products;
        }

        /// <summary>
        /// [DOCUMENT] Update function to update a partial Product document with the given values
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces/{RepositoryDefinition.Keyspace}" +
                $"/collections/{RepositoryDefinition.TableName}/{product.id.ToString()}");

            // Execute API Post 
            var response = Utils.Api.Put(url, GetToken(), JsonConvert.SerializeObject(product));

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The UPDATE Document API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The UPDATE Document API command was sent but failed to be processed correctly by the server with status code: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                throw new Exception(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// [DOCUMENT] Delete function to delete row from Products table
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id)
        {
            //Set API URL
            var url = string.Format($"http://{RepositoryDefinition.Address}:8082" +
                $"/v2/namespaces" +
                $"/{RepositoryDefinition.Keyspace}/collections/{RepositoryDefinition.TableName}/{id}");

            // Execute API Post 
            var response = Utils.Api.Delete(url, GetToken());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The DELETE Document API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return;
            }
            else
            {
                Console.WriteLine(String.Format("The DELETE Document API command was sent but failed to be processed correctly by the server with status code: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                throw new Exception(response.StatusCode.ToString());

            }
        }
    }
}
