using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(UcabGo.Api.Startup))]

namespace UcabGo.Api
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

            //builder.Services.Add(
            //    new ServiceDescriptor(
            //        typeof(IBaseDatabaseService), 
            //        (serviceProvider) => new CentralDatabaseService(connectionString),
            //        ServiceLifetime.Scoped
            //));

            ////Dependency injection
            //builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            //builder.Services.AddTransient<ICallDetailService, CallDetailService>();
        }
    }
}