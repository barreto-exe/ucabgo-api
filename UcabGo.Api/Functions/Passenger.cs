using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Passanger.Dtos;

namespace UcabGo.Api.Functions
{
    public class Passenger
    {
        private readonly IDriverService driverService;
        private readonly IRideService rideService;
        private readonly ApiResponse apiResponse;
        public Passenger(IDriverService driverService, IRideService rideService, ApiResponse apiResponse)
        {
            this.driverService = driverService;
            this.rideService = rideService;
            this.apiResponse = apiResponse;
        }


        #region GetPassengersByRide
        [FunctionName("GetPassengersByRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rides/{rideId:int}/passengers")] HttpRequest req, int rideId, ILogger log)
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
                    switch(ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting passengers by ride. RideId: {ID}", rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region AcceptPassengerRequest
        [FunctionName("AcceptPassengerRequest")]
        [OpenApiOperation(tags: new[] { "Ride" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/{rideId:int}/passengers/{passengerId:int}/accept")] HttpRequest req,
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.AcceptPassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_ACCEPTED";
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
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_NOT_AVAILABLE_OR_ACCEPTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while accepting passenger. RideId: {ID}", rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region IgnorePassengerRequest
        [FunctionName("IgnorePassengerRequest")]
        [OpenApiOperation(tags: new[] { "Ride" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/{rideId:int}/passengers/{passengerId:int}/ignore")] HttpRequest req,
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.IgnorePassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_IGNORED";
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
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_NOT_AVAILABLE_OR_ACCEPTED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while ignoring passenger. RideId: {ID}", rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

        #region CancelPassengerRequest
        [FunctionName("CancelPassengerRequest")]
        [OpenApiOperation(tags: new[] { "Ride" })]
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides/{rideId:int}/passengers/{passengerId:int}/cancel")] HttpRequest req,
            int rideId, int passengerId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await driverService.CancelPassenger(input.Email, rideId, passengerId);
                    apiResponse.Message = "PASSENGER_CANCELLED";
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
                        case "PASSENGER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "REQUEST_ALREADY_CANCELED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while cancelling passenger. RideId: {ID}", rideId);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
