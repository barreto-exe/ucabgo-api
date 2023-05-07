using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using UcabGo.Api.Utils;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
using UcabGo.Core.Interfaces;
using UcabGo.Infrastructure.Data;
using UcabGo.Infrastructure.Repositories;

[assembly: FunctionsStartup(typeof(UcabGo.Api.Startup))]

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

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IAuthService, AuthService>();
        }
    }
}