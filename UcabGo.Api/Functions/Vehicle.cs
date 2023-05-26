using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Vehicle.Dtos;
using UcabGo.Core.Data.Vehicle.Inputs;

namespace UcabGo.Api.Functions
{
    public class Vehicle
    {
        private readonly ApiResponse apiResponse;
        private readonly IVehicleService vehicleService;
        public Vehicle(ApiResponse apiResponse, IVehicleService vehicleService)
        {
            this.apiResponse = apiResponse;
            this.vehicleService = vehicleService;
        }

        #region GetVehicles
        [FunctionName("GetVehicles")]
        [OpenApiOperation(tags: new[] { "Vehicle" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<VehicleDto>),
            Description = "Get the user's vehicles.")]
        #endregion
        public async Task<IActionResult> GetVehicles(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var vehicles = await vehicleService.GetAllDtos(input.Email);

                apiResponse.Message = "VEHICLES_FOUND";
                apiResponse.Data = vehicles;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CreateVehicle
        [FunctionName("CreateVehicle")]
        [OpenApiOperation(tags: new[] { "Vehicle", "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(contentType:
            "application/json",
            bodyType: typeof(VehicleInput),
            Required = true,
            Description = "Creates a user's vehicle.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(VehicleDto),
            Description = "The created vehicle.")]
        #endregion
        public async Task<IActionResult> CreateVehicle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(VehicleInput input)
            {
                try
                {
                    var vehicle = await vehicleService.Create(input);
                    apiResponse.Message = "VEHICLE_CREATED";
                    apiResponse.Data = vehicle;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        default:
                            {
                                log.LogError(ex, "Error while creating vehicle", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<VehicleInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region UpdateVehicle
        [FunctionName("UpdateVehicle")]
        [OpenApiOperation(tags: new[] { "Vehicle" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(VehicleUpdateInput),
            Required = true,
            Description = "Updates a user's vehicle.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(VehicleDto),
            Description = "The data of the updated vehicle.")]
        #endregion
        public async Task<IActionResult> UpdateVehicle(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/vehicles")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(VehicleUpdateInput input)
            {
                try
                {
                    var vehicle = await vehicleService.Update(input);
                    apiResponse.Message = "VEHICLE_UPDATED";
                    apiResponse.Data = vehicle;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "VEHICLE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while updating vehicle.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }

            }

            return await RequestHandler.Handle<VehicleUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region DeleteVehicle
        [FunctionName("DeleteVehicle")]
        [OpenApiOperation(tags: new[] { "Vehicle" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "id",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the vehicle to delete.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(VehicleDto),
            Description = "Info of the deleted vehicle.")]
        #endregion
        public async Task<IActionResult> DeleteVehicle(
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
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "VEHICLE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while deleting vehicle with id {ID}", id);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
