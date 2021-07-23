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
    public class GraphQlProductRepository : IProductRepository
    {
        private readonly IWebHostEnvironment _host;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initialize GraphQL Repository and construct class
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public GraphQlProductRepository(IWebHostEnvironment host, IConfiguration config)
        {
            _host = host;
            _config = config;
        }

        /// <summary>
        /// Get GraphQL repository definition
        /// </summary>
        public RepositoryDefinition RepositoryDefinition
        {
            get
            {
                return AstraDMLDemo.Program._repositoryDefinitions
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.GraphQl.ToString());
            }
        }

        /// <summary>
        /// Checks to validate connectivity to the GraphQL API endpoint
        /// </summary>
        /// <returns></returns>
        public bool IsRepositoryAvailable()
        {
            try
            {
                //Set API URL
                var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
                    $".apps.astra.datastax.com/api/graphql-schema");

                //Set Query
                string graphQlQuery = "{\"query\":\"query GetTables {\\r\\n  keyspace(name: \\\"dml_demo\\\") {\\r\\n    tables {\\r\\n      name\\r\\n    }\\r\\n  }\\r\\n}\",\"variables\":{}}";

                // Execute API Get 
                IRestResponse response = Utils.Api.Post(url, this.RepositoryDefinition.Token, graphQlQuery);

                if (response.IsSuccessful)
                {
                    // Parse results
                    JToken result = JToken.Parse(response.Content);
                    JToken tables = result.SelectToken("data.keyspace.tables");
                    JToken table_names = tables.First();

                    if (table_names.ToString().Contains("products"))
                    {
                        return true; // found a table named products
                    }
                    else
                    {
                        Console.WriteLine(String.Format("GraphQL repository does not include table 'products': {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("GraphQL repository unavailable with status: {0} {1}", response.StatusCode.ToString(), response.Content.ToString()));
                    throw new Exception(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* Implementation of GraphQL CRUD API Operations */

        /// <summary>
        /// [GRAPHQL] Creates new Product row
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Add(Product product)
        {
            //Set API URL
            var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
                $".apps.astra.datastax.com/api/graphql/{RepositoryDefinition.Keyspace}");

            // Execute API Post 
            IRestResponse response = Utils.Api.Post(url, this.RepositoryDefinition.Token, product.GetGraphQlInsertJson());

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The ADD Graph API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The ADD Graph API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception();
            }
        }

        /// <summary>
        /// [GRAPHQL] Read function to get Product with the provided id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProduct(Guid id)
        {
            //Set API URL
            var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
            $".apps.astra.datastax.com/api/graphql/{RepositoryDefinition.Keyspace}");

            //Set Query
            string graphQLQuery = ("{\"query\":\"query dml_demo {\\r\\n    products (value: {id:\\\"" + id.ToString() + "\\\"}) {\\r\\n      values {\\r\\n        id\\r\\n        productname\\r\\n        description\\r\\n        created\\r\\n        price\\r\\n       field1\\r\\n      }\\r\\n    }\\r\\n}\",\"variables\":{}}");

            // Execute API Post 
            var response = Utils.Api.Post(url, this.RepositoryDefinition.Token, graphQLQuery);

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GET Graph API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));

                //Parse results
                JToken result = JToken.Parse(response.Content);
                JToken data = result.SelectToken("data.products.values");

                Product product = JsonConvert.DeserializeObject<Product>(data[0].ToString());

                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The GET Graph API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception();
            }
        }

        /// <summary>
        /// [GRAPHQL] Read function to get all rows in the Products table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProduct()
        {
            //Set API URL
            var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
                $".apps.astra.datastax.com/api/graphql/{RepositoryDefinition.Keyspace}");

            // Create GraphQL query to return all products
            string graphQlQuery = "{\"query\":\"query dml_demo {\\r\\n    products (value: {}) {\\r\\n      " +
                "values {\\r\\n        id\\r\\n        productname\\r\\n        created\\r\\n        price\\r\\n " +
                "       description\\r\\n       field1\\r\\n      }\\r\\n    }\\r\\n}\",\"variables\":{}}";

            // Execute API Post 
            var response = Utils.Api.Post(url, this.RepositoryDefinition.Token, graphQlQuery);

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The GETALL Graph API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));

                // Parse results
                JToken result = JToken.Parse(response.Content);
                JToken data = result.SelectToken("data.products.values");

                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(data.ToString());// new List<Product>();

                return products;
            }
            else
            {
                Console.WriteLine(String.Format("The GETALL Graph API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception();
            }
        }

        /// <summary>
        /// [GRAPHQL] Update function to update a partial Product row with the given values
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            //Set API URL
            var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
                $".apps.astra.datastax.com/api/graphql/{RepositoryDefinition.Keyspace}");

            // Set Query
            product = product.GetCorrectedTimestamp();

            string graphQL = "{\"query\":\"mutation {updaterecord: updateproducts(value: { id: \\\"" + product.id.ToString() + "\\\" productname: \\\"" + product.productname.ToString() + "\\\", description: \\\"" + product.description.ToString() + "\\\", price: \\\"" + product.price.ToString() + "\\\", created: \\\"" +  product.created.ToString("u").Replace(" ", "T") + "\\\" }options: {consistency: LOCAL_QUORUM }) {value {id }}}\",\"variables\":{}}";

            // Execute API Post 
            var response = Utils.Api.Post(url, this.RepositoryDefinition.Token, graphQL);

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The UPDATE Graph API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return product;
            }
            else
            {
                Console.WriteLine(String.Format("The UPDATE Graph API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception();
            }
        }

        /// <summary>
        /// [GRAPHQL] Delete function to delete row from Products table
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id)
        {
            //Set API URL
            var url = string.Format($"https://{RepositoryDefinition.Address}-{RepositoryDefinition.Region}" +
                $".apps.astra.datastax.com/api/graphql/{RepositoryDefinition.Keyspace}");

            // Create GraphQL query to return all products
            string graphQlQuery = "{\"query\":\"mutation deleteAProduct {\\n    deleteproducts (value: { id: \\\"" + id.ToString() + "\\\"}\\n    options: { consistency: LOCAL_QUORUM }\\n    ifExists: true\\n  ) {\\n    value {\\n      id\\n    }\\n  }\\n}\",\"variables\":{}}";

            // Execute API Post 
            var response = Utils.Api.Post(url, this.RepositoryDefinition.Token, graphQlQuery);

            if (response.IsSuccessful)
            {
                Console.WriteLine(String.Format("The DELETE Graph API command was successfully sent and processed with status code: {0}", response.StatusCode.ToString()));
                return;
            }
            else
            {
                Console.WriteLine(String.Format("The DELETE Graph API command was sent but failed to be processed correctly by the server with status code: {0}", response.StatusCode.ToString()));
                throw new Exception();
            }
        }
    }
}
