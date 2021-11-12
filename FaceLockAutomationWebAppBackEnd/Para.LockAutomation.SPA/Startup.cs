using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using NSwag.AspNetCore;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Service;
using Para.LockAutomation.Utils;

namespace LockAutomation_WA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddDbContext<LockAutomationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnection")));

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore);


            services.AddOptions();
            services.Configure<AzureStorageConfig>(Configuration.GetSection("AzureStorage"));


            services.AddScoped<AzureStorageService>();
            services.AddScoped(c2d => 
                ServiceClient.CreateFromConnectionString(Configuration.GetConnectionString("IoTConnection")));
            services.AddScoped(f =>
            {
                return new FaceServiceClient(
                    Configuration["MSFaceApi:APIKey"],
                    Configuration["MSFaceApi:Endpoint"]
                    );
            });

            services.AddScoped<PersonGroupRepository>();
            services.AddScoped<PersonRepository>();
            services.AddScoped<FaceRepository>();
            services.AddScoped<FaceLogRepository>();

            services.AddScoped<PersonGroupService>();
            services.AddScoped<PersonService>();
            services.AddScoped<FaceService>();
            services.AddScoped<FaceLogService>();
            services.AddScoped<TrainingService>();
            services.AddScoped<IoTControlService>();

            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddSwaggerDocument(config =>
            {
                config.IgnoreObsoleteProperties = true;
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                };

                config.DefaultPropertyNameHandling = NJsonSchema.PropertyNameHandling.CamelCase;
                config.Title = "Lock automation through face recognition";
                config.Description = "An ASP.NET Core web API for loging and traning Face API";
            });
            services.AddAutoMapper();
            services.AddHttpClient<ParaFuncService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCors(builder => {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUi3(swaggerSettings =>
                {
                    swaggerSettings.DocExpansion = "list";
                });
            }
            else
            {
                app.UseCors(builder => {
                    builder.WithOrigins(Configuration["CorsOrigin"].ToString())
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
