using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;
using UcabGo.Core.Interfaces;

namespace UcabGo.Api.Functions.Vehicle
{
    public class CreateVehicle
    {
        private readonly ApiResponse apiResponse;
        private readonly IVehicleService vehicleService;
        public CreateVehicle(ApiResponse apiResponse, IVehicleService vehicleService)
        {
            this.apiResponse = apiResponse;
            this.vehicleService = vehicleService;
        }

        [FunctionName("CreateVehicle")]
        [OpenApiOperation(tags: new[] { "Vehicle", "User" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(VehicleInput), Required = true, Description = "Creates a user's vehicle.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(VehicleDto), Description = "The created vehicle.")]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(VehicleInput input)
            {
                var vehicle = await vehicleService.Create(input);
                apiResponse.Message = "VEHICLE_CREATED";
                apiResponse.Data = vehicle;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<VehicleInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }

}
