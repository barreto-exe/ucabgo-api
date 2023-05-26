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
        private readonly IPassengerService passengerService;
        private readonly ApiResponse apiResponse;
        public Passenger(IPassengerService passengerService, ApiResponse apiResponse)
        {
            this.passengerService = passengerService;
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
                    var dtos = await passengerService.GetPassengersByRide(rideId);
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

    }
}
