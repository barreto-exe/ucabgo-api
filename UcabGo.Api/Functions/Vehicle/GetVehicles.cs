using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Vehicle.Dtos;

namespace UcabGo.Api.Functions.Vehicle
{
    public class GetVehicles
    {
        private readonly ApiResponse apiResponse;
        private readonly IVehicleService vehicleService;
        public GetVehicles(ApiResponse apiResponse, IVehicleService vehicleService)
        {
            this.apiResponse = apiResponse;
            this.vehicleService = vehicleService;
        }

        [FunctionName("GetVehicles")]
        [OpenApiOperation(tags: new[] { "Vehicle", "User" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<VehicleDto>), Description = "Get the user's vehicles.")]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var vehicles = await vehicleService.GetAll(input.Email);
                apiResponse.Message = "VEHICLES_FOUND";
                apiResponse.Data = vehicles;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: true);
        }
    }

}
