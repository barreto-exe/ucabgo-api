using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.Ride.Inputs;

namespace UcabGo.Api.Functions
{
    public class Ride
    {
        private readonly ApiResponse apiResponse;
        private readonly IRideService rideService;
        public Ride(ApiResponse response, IRideService rideService)
        {
            this.apiResponse = response;
            this.rideService = rideService;
        }


        #region GetRides
        [FunctionName("GetRides")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(RideFilter.OnlyAvailable),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(bool),
            Description = "If true, only available rides will be returned.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<RideDto>),
            Description = "The information of the user's rides.")]
        #endregion
        public async Task<IActionResult> GetRides(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rides")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideFilter input)
            {
                var rides = await rideService.GetAll(input.Email, input.OnlyAvailable);
                apiResponse.Message = "RIDES_FOUND";
                apiResponse.Data = rides;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<RideFilter>(req, log, apiResponse, Action, isAnonymous: true);
        }


        #region CreateRide
        [FunctionName("CreateRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(RideInput),
            Required = true,
            Description = "The information of the Ride to be created.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(RideDto),
            Description = "The information of the created Ride.")]
        #endregion
        public async Task<IActionResult> CreateRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "rides")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideInput input)
            {
                try
                {
                    var dto = await rideService.Create(input);
                    apiResponse.Message = "RIDE_CREATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "ACTIVE_RIDE_FOUND":
                            return new BadRequestObjectResult(apiResponse);
                        case "VEHICLE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "DESTINATION_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while creating a ride.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<RideInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region StartRide
        [FunctionName("StartRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(RideAvailableInput),
            Required = true,
            Description = "Let the user start a previusly created ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(RideDto),
            Description = "The data of the ride.")]
        #endregion
        public async Task<IActionResult> StartRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/start")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await rideService.StartRide(input);
                    apiResponse.Message = "RIDE_STARTED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "CANT_START_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while starting a ride.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region CompleteRide
        [FunctionName("CompleteRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(RideAvailableInput),
            Required = true,
            Description = "Let the user complete a previosuly started ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(RideDto),
            Description = "The data of the ride.")]
        #endregion
        public async Task<IActionResult> CompleteRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/complete")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await rideService.CompleteRide(input);
                    apiResponse.Message = "RIDE_COMPLETED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "CANT_COMPLETE_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while completing a ride.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region CancelRide
        [FunctionName("CancelRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(RideAvailableInput),
            Required = true,
            Description = "Let the user cancel a created -but yet not started- ride. You can't cancel a ride that has already started.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(RideDto),
            Description = "The data of the ride.")]
        #endregion
        public async Task<IActionResult> CancelRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/cancel")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await rideService.CancelRide(input);
                    apiResponse.Message = "RIDE_CANCELED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "CANT_CANCEL_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while canceling a ride.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
