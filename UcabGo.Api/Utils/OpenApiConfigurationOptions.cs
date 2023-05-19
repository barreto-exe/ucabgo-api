using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using System;

namespace UcabGo.Api.Utils
{
    internal class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {
        public override OpenApiInfo Info { get; set; } = new OpenApiInfo
        {
            Version = "1.0.0",
            Title = "UcabGo API",
            Description = "Ride-sharing platform for the UCAB community",
            License = new OpenApiLicense
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };

        public override OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V3;
    }
}
