using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace UcabGo.Api.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServicesFromAssembly(this IServiceCollection services, Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                // Filter only assemblies that have a reference to UcabGo
                if (!assembly.FullName.Contains("UcabGo"))
                {
                    continue;
                }

                // Get all public types from the assembly
                var interfaceTypes = assembly
                    .GetExportedTypes()
                    .Where(x => x.IsInterface && x.Name.StartsWith("I"));

                foreach (var interfaceType in interfaceTypes)
                {
                    // Get the name of the type that implements the interface
                    var implementationTypeName = interfaceType.Name.Substring(1);

                    // Get the type that implements the interface from the exported types of the assemblies array
                    var implementationsList = assemblies.Select(x =>
                         x.GetExportedTypes()
                        .Where(x => x.IsClass && x.Name == implementationTypeName)
                        .FirstOrDefault());
                    var implementation = implementationsList?.FirstOrDefault(x => x.IsClass && x.Name == implementationTypeName);

                    if (implementationsList != null && implementation != null)
                    {
                        services.AddTransient(interfaceType, implementation);
                    }
                }
            }
        }
    }
}
