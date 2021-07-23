using AstraDMLDemo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Repositories;

namespace AstraDMLDemo
{
    public class Program
    {
        public delegate IProductRepository ServiceResolver(string key);
        public static List<Product> _productList = new List<Product>();
        public static List<RepositoryDefinition> _repositoryDefinitions = new List<RepositoryDefinition>();
        public static string _connectionType;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
