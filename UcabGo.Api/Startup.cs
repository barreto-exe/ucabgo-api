using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Reflection;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
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
            builder.Services.AddTransient<IDestinationService, DestinationService>();
            builder.Services.AddTransient<IRideService, RideService>();
            builder.Services.AddTransient<ILocationService, LocationService>();
            builder.Services.AddTransient<IPassengerService, PassengerService>();
            builder.Services.AddTransient<IDriverService, DriverService>();
            builder.Services.AddTransient<IChatService, ChatService>();

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