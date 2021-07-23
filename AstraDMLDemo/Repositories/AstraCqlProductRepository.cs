using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AstraDMLDemo.Models;

//Debug
using System.Diagnostics;

namespace AstraDMLDemo.Repositories
{
    public class AstraCqlProductRepository : IProductRepository
    {
        public ISession _Session;
        public IMapper _Mapper;

        /// <summary>
        /// [CQL] Initializes Astra CQL Product Repository and Establishes connection
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public AstraCqlProductRepository(IWebHostEnvironment host, IConfiguration config)
        {
            string dirSeparator = Path.DirectorySeparatorChar.ToString();
            string secureConnectionBundle = host.ContentRootPath + dirSeparator + this.RepositoryDefinition.Address;

            //Debug
            // Specify the minimum trace level you want to see
            //Cassandra.Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Info;

            //// Add a standard .NET trace listener
            //Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            try
            {
                if (_Session == null)
                {
                    //Build connection to Astra and establish connection
                    _Session = (Session)Cluster.Builder()
                                           .WithCloudSecureConnectionBundle(secureConnectionBundle)
                                           .WithCredentials(this.RepositoryDefinition.UserClientId, this.RepositoryDefinition.PasswordSecret)
                                           .Build()
                                           .Connect(this.RepositoryDefinition.Keyspace);

                }
                if (_Mapper == null)
                {
                    _Mapper = new Mapper(_Session);

                    try
                    {
                        //Define Cassandra Mapper Configuration 
                        MappingConfiguration.Global.Define(
                            new Map<Product>()
                                .TableName(this.RepositoryDefinition.TableName)
                                .PartitionKey(u => u.id)
                            );
                    }
                    catch (Exception ex)
                    {
                        // do nothing 
                    }
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public RepositoryDefinition RepositoryDefinition
        {
            get
            {
                return AstraDMLDemo.Program._repositoryDefinitions
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.AstraCql.ToString());
            }
        }

        /// <summary>
        /// [CQL] Checks if repository is available and responding to queries
        /// </summary>
        /// <returns></returns>
        public bool IsRepositoryAvailable()
        {
            try
            {
                var row = _Session.Execute("select table_name from system_schema.tables where keyspace_name = 'dml_demo' and table_name='products'").First();
                return true; // assumes that data was returned and connection is available

            } catch (Exception ex)
            {
                return false;
            }
        }


        /*CRUD Function Implementations*/

        /// <summary>
        /// [CQL] Create function to add row to Product table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Add(Product product)
        {
            try
            {
                _Mapper.Insert(product);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CQL Add operation failed with exception: {ex}");
            }

            return product;
        }

        /// <summary>
        /// [CQL] Gets all rows in the Product table
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProduct()
        {
            IEnumerable<Product> products = (IEnumerable<Product>)_Mapper.Fetch<Product>("SELECT * FROM products");

            return products.ToList();
        }


        /// <summary>
        /// [CQL] Read function to get row from Product table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProduct(Guid id)
        {
            try
            {
                Product product = _Mapper.Single<Product>("WHERE id=?", id);
                return _Mapper.Single<Product>("WHERE id=?", id);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in GetProduct - " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// [CQL] Update function to update row in Product table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            _Mapper.Update(product);

            return product;
        }

        /// <summary>
        /// [CQL] Delete function to remove row from Product table
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id)
        {
            Product product = this.GetProduct(id);

            _Mapper.Delete(product);

            return;
        }
    }
}
