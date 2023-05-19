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
    public class DeleteVehicle
    {
        private readonly ApiResponse apiResponse;
        private readonly IVehicleService vehicleService;
        public DeleteVehicle(ApiResponse apiResponse, IVehicleService vehicleService)
        {
            this.apiResponse = apiResponse;
            this.vehicleService = vehicleService;
        }

        [FunctionName("DeleteVehicle")]
        [OpenApiOperation(tags: new[] { "User", "Vehicle" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The ID of the vehicle to delete.")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(VehicleDto), Description = "Info for the deleted vehicle.")]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/vehicles/{id:int}")] HttpRequest req, int id, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await vehicleService.Delete(input.Email, id);
                    apiResponse.Message = "VEHICLE_DELETED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch(Exception e)
                {
                    apiResponse.Message = e.Message;
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }

}
