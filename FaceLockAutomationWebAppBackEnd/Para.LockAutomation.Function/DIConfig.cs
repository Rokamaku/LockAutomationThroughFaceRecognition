using Autofac;
using AutoMapper;
using AzureFunctions.Autofac.Configuration;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.Devices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.ProjectOxford.Face;
using Para.LockAutomation.Models;
using Para.LockAutomation.Repository;
using Para.LockAutomation.Service;
using Para.LockAutomation.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Para.LockAutomation.Function
{
    public class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.Register<IConfiguration>(context =>
                {
                    return new ConfigurationBuilder()
                        .SetBasePath(Environment.CurrentDirectory)
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables()
                        .Build();
                }).InstancePerLifetimeScope();

                builder.Register(context => 
                {
                    IConfiguration _config = context.Resolve<IConfiguration>();
                    return new LockAutomationDbContext(new DbContextOptionsBuilder<LockAutomationDbContext>()
                        .UseSqlServer(_config.GetConnectionString("SqlConnection")).Options);
                }).InstancePerLifetimeScope();

                builder.Register(context =>
                {
                    IConfiguration _config = context.Resolve<IConfiguration>();
                    return ServiceClient.CreateFromConnectionString(_config.GetConnectionString("IoTConnection"));
                }).InstancePerLifetimeScope();

                builder.Register(context =>
                {
                    IConfiguration _config = context.Resolve<IConfiguration>();
                    return new TelemetryClient()
                    {
                        InstrumentationKey = TelemetryConfiguration.Active.InstrumentationKey = 
                            _config["APPINSIGHTS_INSTRUMENTATIONKEY"]
                    };
                }).InstancePerLifetimeScope();

                builder.Register(context =>
                {
                    IConfiguration _config = context.Resolve<IConfiguration>();
                    return new FaceServiceClient(
                        _config["MSFaceApi:APIKey"],
                        _config["MSFaceApi:Endpoint"]
                        );
                }).InstancePerLifetimeScope();

                builder.Register(context =>
                {
                    var _config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile(new AutoMapperProfileConfiguration());
                    });

                    return _config.CreateMapper();
                });

                builder.Register(context => {
                    return Options.Create(new AzureStorageConfig());
                });

                builder.Register(context =>
                {
                    IConfiguration _config = context.Resolve<IConfiguration>();
                    return new ParaFuncService(HttpClientFactory.Create(), _config);
                });

                builder.RegisterType<AzureStorageService>();

                builder.RegisterType<PersonGroupRepository>();
                builder.RegisterType<PersonRepository>();
                builder.RegisterType<FaceRepository>();
                builder.RegisterType<FaceLogRepository>();     

                builder.RegisterType<PersonGroupService>();
                builder.RegisterType<PersonService>();
                builder.RegisterType<FaceService>();
                builder.RegisterType<FaceLogService>();
                builder.RegisterType<TrainingService>();

            }, functionName);
        }
    }
}
