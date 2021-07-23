using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Mapping;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using AstraDMLDemo.Models;

//Debug
using System.Diagnostics;

namespace AstraDMLDemo.Repositories
{
    public class DseCqlProductRepository : IProductRepository
    {
        ISession Session;
        IMapper Mapper;

        /// <summary>
        /// [CQL] Initializes DSE CQL Product Repository and Establishes connection
        /// </summary>
        /// <param name="host"></param>
        /// <param name="config"></param>
        public DseCqlProductRepository(IWebHostEnvironment host, IConfiguration config)
        {
            //Debug
            // Specify the minimum trace level you want to see
            Cassandra.Diagnostics.CassandraTraceSwitch.Level = TraceLevel.Info;

            // Add a standard .NET trace listener
            Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

            try
            {
                if (Session == null)
                {
                    //Build connection to DSE and establish connection
                    Session = (Session) Cluster.Builder()
                        .AddContactPoint(this.RepositoryDefinition.Address)
                        .WithCredentials(
                            this.RepositoryDefinition.UserClientId,
                            this.RepositoryDefinition.PasswordSecret)
                        .Build()
                        .Connect(this.RepositoryDefinition.Keyspace);

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
                if (Mapper == null)
                {
                    Mapper = new Mapper(Session);
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
                .FirstOrDefault(rd => rd.Name == Enums.RepositoryType.DseCql.ToString());
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
                var row = Session.Execute("select table_name from system_schema.tables where keyspace_name = 'dml_demo' and table_name='products'").First();
                return true; // assumes that data was returned and connection is available

            }
            catch (Exception ex)
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
                //Execute CQL command an set Consistency Level to LocalQuorum (Default Local_ONE not allowed)
                Mapper.Insert(product, new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));
            } catch (Exception ex)
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
            IEnumerable<Product> products = (IEnumerable<Product>) 
                Mapper.Fetch<Product>("SELECT * FROM products");

            return products.ToList();
        }


        /// <summary>
        /// [CQL] Read function to get row from Product table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Product GetProduct(Guid id)
        {
            return Mapper.Single<Product>("WHERE id=?", id);
        }

        /// <summary>
        /// [CQL] Update function to update row in Product table
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Product Update(Product product)
        {
            //Execute CQL command an set Consistency Level to LocalQuorum (Default Local_ONE not allowed)
            Mapper.Update(product, new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

            return product;
        }

        /// <summary>
        /// [CQL] Delete function to remove row from Product table
        /// </summary>
        /// <param name="id"></param>
        public void Delete(Guid id)
        {
            Product product = this.GetProduct(id);

            //Execute CQL command an set Consistency Level to LocalQuorum (Default Local_ONE not allowed)
            Mapper.Delete(product, new CqlQueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalQuorum));

            return;
        }
    }
}
