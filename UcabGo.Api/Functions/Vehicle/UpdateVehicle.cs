using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;

namespace UcabGo.Api.Functions.Vehicle
{
    public class UpdateVehicle
    {
        private readonly ApiResponse apiResponse;
        private readonly IVehicleService vehicleService;
        public UpdateVehicle(ApiResponse apiResponse, IVehicleService vehicleService)
        {
            this.apiResponse = apiResponse;
            this.vehicleService = vehicleService;
        }

        [FunctionName("UpdateVehicle")]
        [OpenApiOperation(tags: new[] { "User", "Vehicle" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(VehicleUpdateInput), Required = true, Description = "Updates a user's vehicle.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(VehicleDto), Description = "The data of the updated vehicle.")]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(VehicleUpdateInput input)
            {
                var vehicle = await vehicleService.Update(input);
                apiResponse.Message = "VEHICLE_UPDATED";
                apiResponse.Data = vehicle;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<VehicleUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }

}
