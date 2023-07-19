using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Reflection;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
using UcabGo.Application.Utils;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;
using UcabGo.Infrastructure.Repositories;

[assembly: WebJobsStartup(typeof(UcabGo.Api.Startup))]
namespace UcabGo.Api
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Database Connection
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<UcabgoContext>(options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            //Register DTOs mappings and services
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            builder.Services.AddAutoMapper(assemblies);
            //builder.Services.AddServicesFromAssembly(assemblies);

            //Dependency injection
            builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
            builder.Services.AddTransient<ApiResponse>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IVehicleService, VehicleService>();
            builder.Services.AddTransient<ISoscontactService, SoscontactService>();
            builder.Services.AddTransient<IRideService, RideService>();
            builder.Services.AddTransient<ILocationService, LocationService>();
            builder.Services.AddTransient<IPassengerService, PassengerService>();
            builder.Services.AddTransient<IDriverService, DriverService>();
            builder.Services.AddTransient<IChatService, ChatService>();
            builder.Services.AddTransient<IEvaluationService, EvaluationService>();
            builder.Services.AddTransient<IMailService, MailService>();

            builder.Services.AddTransient<MailSettings>((provider) =>
            {
                return new MailSettings
                {
                    Server = Environment.GetEnvironmentVariable("EmailServer"),
                    Port = Convert.ToInt32(Environment.GetEnvironmentVariable("EmailPort")),
                    SenderName = Environment.GetEnvironmentVariable("SenderName"),
                    SenderEmail = Environment.GetEnvironmentVariable("SenderEmail"),
                    UserName = Environment.GetEnvironmentVariable("SenderEmail"),
                    Password = Environment.GetEnvironmentVariable("Password"),
                };
            });

            //Swagger
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.AddCodeParameter = true;
                opts.Documents = new[] {
                    new SwaggerDocument {
                        Name = "v1",
                            Title = "Swagger document",
                            Description = "Integrate Swagger UI With Azure Functions",
                            Version = "v2"
                    }
                };
                opts.ConfigureSwaggerGen = x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo mInfo) ? mInfo.Name : default(Guid).ToString();
                    });
                };
            });
        }
    }
}