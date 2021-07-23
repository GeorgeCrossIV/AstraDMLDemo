using AstraDMLDemo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AstraDMLDemo.Repositories;
using static AstraDMLDemo.Program;

namespace AstraDMLDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            // load the Repository Definitions from the Json file 
            string repositoriesFile = env.ContentRootPath + "/RepositoryDefinitions.json";
            string repositoryDefinitionsJson = File.ReadAllText(repositoriesFile);
            _repositoryDefinitions = JsonConvert.DeserializeObject <List<RepositoryDefinition>>(repositoryDefinitionsJson);

            // get the connection type to start with from the configuration file
            _connectionType = Configuration.GetSection("Astra").GetSection("ConnectionType").Value;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionType = Configuration.GetSection("Astra").GetSection("ConnectionType").Value;

            services.AddControllersWithViews();

            // register the repositories used in the application
            services.AddTransient<AstraCqlProductRepository>();
            services.AddTransient<DseCqlProductRepository>();
            services.AddTransient<RestProductRepository>();
            services.AddTransient<GraphQlProductRepository>();
            services.AddTransient<MockProductRepository>();
            services.AddTransient<DocumentProductRepository>();
            services.AddTransient<DseRestProductRepository>();
            services.AddTransient<DseDocumentProductRepository>();
            services.AddTransient<DseGraphQlProductRepository>();

            services.AddTransient<ServiceResolver>(ServiceProvider => key =>
            { //TODO - replace the string key with the enumeration
                switch (key)
                {
                    case "AstraCql":
                        return ServiceProvider.GetService<AstraCqlProductRepository>();
                    case "DseCql":
                        return ServiceProvider.GetService<DseCqlProductRepository>();
                    case "Rest":
                        return ServiceProvider.GetService<RestProductRepository>();
                    case "GraphQl":
                        return ServiceProvider.GetService<GraphQlProductRepository>();
                    case "Memory":
                        return ServiceProvider.GetService<MockProductRepository>();
                    case "Document":
                        return ServiceProvider.GetService<DocumentProductRepository>();
                    case "DseRest":
                        return ServiceProvider.GetService<DseRestProductRepository>();
                    case "DseGraphQl":
                        return ServiceProvider.GetService<DseGraphQlProductRepository>();
                    case "DseDocument":
                        return ServiceProvider.GetService<DseDocumentProductRepository>();
                    default:
                        throw new KeyNotFoundException();
                }
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Product/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Product}/{action=Index}/{id?}");
            });
        }
    }
}
