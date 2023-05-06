using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
using UcabGo.Core.Interfaces;
using UcabGo.Core.Repositories;
using UcabGo.Infrastructure.Data;

[assembly: FunctionsStartup(typeof(UcabGo.Api.Startup))]

namespace UcabGo.Api
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //Database Connection
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");
            builder.Services.AddDbContext<UcabgoContext>(options => options.UseMySQL(connectionString));

            //Dependency injection
            builder.Services.AddTransient<IUserRepository, UserRepository>();
            builder.Services.AddTransient<IAuthService, AuthService>();
        }
    }
}