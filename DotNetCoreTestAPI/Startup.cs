using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DotNetCoreTestAPILib.DAL;
using DotNetCoreTestAPILib.BLL;
using Swashbuckle.AspNetCore.Swagger;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using DotNetCoreTestAPI.BLL.Interfaces;
using DotNetCoreTestAPI.DAL.Interfaces;

namespace DotNetCoreTestAPI
{
    public class Startup
    {
        // setup the in memory database.
        private const string _DB_NAME = "dpt-net-core-test-api";

        private static IInMemoryRepository _db;

        static Startup() => _db = new InMemoryRepository(_DB_NAME);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Setup dipendency injection
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(_db).As<IInMemoryRepository>();
            builder.RegisterType<VehicleOperations>().As<IVehicleOperations>();
            builder.RegisterType<DataAccessLayer>().As<IDataAccessLayer>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddMvc() //x => x.OutputFormatters.Add(new Microsoft.AspNetCore.Mvc.Formatters.XmlSerializerOutputFormatter()));
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());



            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = ".NET Core Test API",
                    Description = "Simple Vehicle Records Web API Service",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Jesse Myers",
                        Email = "xxx@xxx.com"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();


            }

            // make sure our in-memory db gets properly disposed and persisted upon shutdown
            applicationLifetime.ApplicationStopping.Register(() => _db.Dispose());

            app.UseCors(builder => builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());

            app.UseMvc();

            app.UseSwagger();

            // Swagger UI is available at the root URL eg. http://localhost:5000
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ".NET Core Test API");
                c.RoutePrefix = string.Empty;
            });



        }
    }
}
