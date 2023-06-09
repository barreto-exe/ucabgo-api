using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Passanger.Inputs;
using UcabGo.Core.Data.Passenger.Dtos;
using UcabGo.Core.Data.Passenger.Inputs;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;

namespace UcabGo.Api.Functions
{
    public class Passenger
    {
        private readonly IPassengerService passengerService;
        private readonly ApiResponse apiResponse;
        public Passenger(IPassengerService passengerService, ApiResponse apiResponse)
        {
            this.passengerService = passengerService;
            this.apiResponse = apiResponse;
        }


        #region AskForRide
        [FunctionName("AskForRide")]
        [OpenApiOperation(tags: new[] { "Passenger" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(PassengerInput),
            Required = true,
            Description = "The information from the passenger in order to ask for a ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The data from the passenger that asked for a ride.")]
        #endregion
        public async Task<IActionResult> AskForRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "passenger/rides/ask")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            async Task<IActionResult> Action(PassengerInput input)
            {
                try
                {
                    var dto = await passengerService.AskForRide(input);
                    apiResponse.Message = "ASKED_FOR_RIDE";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(AskForRide),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Ride },
                        log);

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "LOCATION_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "NO_AVAILABLE_SEATS":
                            return new BadRequestObjectResult(apiResponse);
                        case "ALREADY_IN_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while asking ride.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<PassengerInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region GetRides
        [FunctionName("GetRides")]
        [OpenApiOperation(tags: new[] { "Passenger" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(RideFilter.OnlyAvailable),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(bool),
            Description = "If true, only the active ride will be returned. A user can have only one active ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<RideDto>),
            Description = "The information of the passenger's ride(s).")]
        #endregion
        public async Task<IActionResult> GetRides(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passenger/rides")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideFilter input)
            {
                try
                {
                    var dto = await passengerService.GetRides(input);
                    apiResponse.Message = "RIDES_FOUND";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        default:
                            {
                                log.LogError(ex, "Error while getting passenger's current ride. \n" +  ex.Message + "\n" + ex.StackTrace, input.Email);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<RideFilter>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CancelRideByPassenger
        [FunctionName("CancelRideByPassenger")]
        [OpenApiOperation(tags: new[] { "Passenger" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(CancelRideInput),
            Required = true,
            Description = "The information for canceling the ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The information of the passenger that canceled the ride.")]
        #endregion
        public async Task<IActionResult> CancelRideByPassenger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "passenger/cancel")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            async Task<IActionResult> Action(CancelRideInput input)
            {
                try
                {
                    var dto = await passengerService.CancelRide(input);
                    apiResponse.Message = "RIDE_CANCELED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(CancelRideByPassenger),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Ride },
                        log);

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "NOT_IN_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while canceling ride.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<CancelRideInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region FinishRideByPassenger
        [FunctionName("FinishRideByPassenger")]
        [OpenApiOperation(tags: new[] { "Passenger" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(FinishRideInput),
            Required = true,
            Description = "The information for finishing a ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The information of the user that have succesfully exited a ride.")]
        #endregion
        public async Task<IActionResult> FinishRideByPassenger(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "passenger/finish")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            async Task<IActionResult> Action(FinishRideInput input)
            {
                try
                {
                    var dto = await passengerService.FinishRide(input);
                    apiResponse.Message = "RIDE_FINISHED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(FinishRideByPassenger),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Ride },
                        log);

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "NOT_IN_RIDE":
                            return new BadRequestObjectResult(apiResponse);
                        case "RIDE_NOT_STARTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while finishing ride.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<FinishRideInput>(req, log, apiResponse, Action, isAnonymous: false);
        }



        #region GetPassengerCooldown
        [FunctionName("GetPassengerCooldown")]
        [OpenApiOperation(tags: new[] { "Passenger" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(CooldownDto),
            Description = "ResponseBodyDescription")]
        #endregion
        public async Task<IActionResult> GetPassengerCooldown(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passenger/cooldown")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await passengerService.GetPassengerCooldownTime(input.Email);
                    apiResponse.Message = "COOLDOWN_TIME";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        default:
                            {
                                log.LogError(ex, "Error while getting passenger's cooldown.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

    }
}
