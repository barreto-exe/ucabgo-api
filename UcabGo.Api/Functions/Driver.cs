using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Passanger.Dtos;
using UcabGo.Core.Data.Ride.Dtos;
using UcabGo.Core.Data.Ride.Filters;
using UcabGo.Core.Data.Ride.Inputs;

namespace UcabGo.Api.Functions
{
    public partial class Driver
    {
        private readonly IDriverService driverService;
        private readonly IRideService rideService;
        private readonly ApiResponse apiResponse;
        public Driver(IDriverService driverService, IRideService rideService, ApiResponse apiResponse)
        {
            this.driverService = driverService;
            this.rideService = rideService;
            this.apiResponse = apiResponse;
        }


        #region GetDriverRides
        [FunctionName("GetDriverRides")]
        [OpenApiOperation(tags: new[] { "Driver" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(RideFilter.OnlyAvailable),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(bool),
            Description = "If true, only the available ride will be returned. A user can have only one available ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<RideDto>),
            Description = "The information of the user's rides.")]
        #endregion
        public async Task<IActionResult> GetRides(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "driver/rides")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideFilter input)
            {
                var rides = await rideService.GetAll(input.Email, input.OnlyAvailable);
                apiResponse.Message = "RIDES_FOUND";
                apiResponse.Data = rides;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<RideFilter>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CreateRide
        [FunctionName("CreateRide")]
        [OpenApiOperation(tags: new[] { "Driver" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "driver/rides/create")] HttpRequest req, 
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            async Task<IActionResult> Action(RideInput input)
            {
                try
                {
                    var dto = await driverService.CreateRide(input);
                    apiResponse.Message = "RIDE_CREATED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Id });

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "ACTIVE_RIDE_FOUND":
                            return new BadRequestObjectResult(apiResponse);
                        case "SEAT_LIMIT_REACHED":
                            return new BadRequestObjectResult(apiResponse);
                        case "VEHICLE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "DESTINATION_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while creating a ride.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<RideInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region StartRide
        [FunctionName("StartRide")]
        [OpenApiOperation(tags: new[] { "Driver" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/rides/start")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> ridesMatchingHub,
            ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await driverService.StartRide(input);
                    apiResponse.Message = "RIDE_STARTED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(StartRide),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Id },
                        log);
                    await ridesMatchingHub.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Id });

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
                                log.LogError(ex, "Error while starting a ride.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CompleteRide
        [FunctionName("CompleteRide")]
        [OpenApiOperation(tags: new[] { "Driver" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/rides/complete")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> ridesMatchingHub,
            ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await driverService.CompleteRide(input);
                    apiResponse.Message = "RIDE_COMPLETED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(CompleteRide),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Id },
                        log);
                    await ridesMatchingHub.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Id });

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
                                log.LogError(ex, "Error while completing a ride.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CancelRide
        [FunctionName("CancelRide")]
        [OpenApiOperation(tags: new[] { "Driver" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/rides/cancel")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> ridesMatchingHub,
            ILogger log)
        {
            async Task<IActionResult> Action(RideAvailableInput input)
            {
                try
                {
                    var dto = await driverService.CancelRide(input);
                    apiResponse.Message = "RIDE_CANCELED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(CancelRide),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Id }, 
                        log);
                    await ridesMatchingHub.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Id });

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
                                log.LogError(ex, "Error while canceling a ride.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<RideAvailableInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }

    //Portion for the passengers of the ride
    public partial class Driver
    {
        #region GetPassengersByRide
        [FunctionName("GetPassengersByRide")]
        [OpenApiOperation(tags: new[] { "Driver" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride to consult.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<PassengerDto>),
            Description = "A list with the information of the passengers asking for this ride.")]
        #endregion
        public async Task<IActionResult> GetPassengersByRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "driver/{rideId:int}/passengers")] HttpRequest req, int rideId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dtos = await rideService.GetPassengers(rideId);
                    apiResponse.Message = "PASSENGERS_FOUND";
                    apiResponse.Data = dtos;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting passengers by ride. RideId: {ID}\n" + ex.Message + "\n" + ex.StackTrace, rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region AcceptPassengerRequest
        [FunctionName("AcceptPassengerRequest")]
        [OpenApiOperation(tags: new[] { "Driver" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride to consult.")]
        [OpenApiParameter(
            name: "passengerId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the passenger request to accept.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The information of the accepted passenger.")]
        #endregion
        public async Task<IActionResult> AcceptPassengerRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/{rideId:int}/passengers/{passengerId:int}/accept")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> ridesMatchingHub,
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.AcceptPassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_ACCEPTED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(AcceptPassengerRequest),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Ride },
                        log);
                    await ridesMatchingHub.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Ride });

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_NOT_AVAILABLE_OR_ACCEPTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while accepting passenger. RideId: {ID}\n" + ex.Message + "\n" + ex.StackTrace, rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region IgnorePassengerRequest
        [FunctionName("IgnorePassengerRequest")]
        [OpenApiOperation(tags: new[] { "Driver" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride to consult.")]
        [OpenApiParameter(
            name: "passengerId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the passenger request to ignore.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The information of the ignored passenger.")]
        #endregion
        public async Task<IActionResult> IgnorePassengerRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/{rideId:int}/passengers/{passengerId:int}/ignore")] HttpRequest req, 
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages, 
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.IgnorePassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_IGNORED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(IgnorePassengerRequest),
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
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_NOT_AVAILABLE_OR_ACCEPTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while ignoring passenger. RideId: {ID}\n" + ex.Message + "\n" + ex.StackTrace, rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CancelPassengerRequest
        [FunctionName("CancelPassengerRequest")]
        [OpenApiOperation(tags: new[] { "Driver" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride to consult.")]
        [OpenApiParameter(
            name: "passengerId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the passenger request to cancel.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(PassengerDto),
            Description = "The information of the cancelled passenger.")]
        #endregion
        public async Task<IActionResult> CancelPassengerRequest(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "driver/{rideId:int}/passengers/{passengerId:int}/cancel")] HttpRequest req,
            [SignalR(HubName = "activeride")] IAsyncCollector<SignalRMessage> signalRMessages,
            [SignalR(HubName = "ridesmatching")] IAsyncCollector<SignalRMessage> ridesMatchingHub,
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.CancelPassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_CANCELLED";
                    apiResponse.Data = dto;

                    await signalRMessages.Send(
                        nameof(CancelPassengerRequest),
                        input.Email,
                        HubRoutes.ACTIVE_RIDE_RECEIVE_UPDATE, 
                        dto.UsersToMessage, 
                        new object[] { dto.Ride },
                        log);
                    await ridesMatchingHub.Send(HubRoutes.RIDES_MATCHING_RECEIVE_UPDATE, new object[] { dto.Ride });

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_ALREADY_CANCELED_OR_NOT_ACCEPTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while cancelling passenger. RideId: {ID}\n" + ex.Message + "\n" + ex.StackTrace, rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
