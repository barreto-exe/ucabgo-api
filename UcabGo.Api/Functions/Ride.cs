using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Ride.Filters;

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


        #region GetMatchingRides
        [FunctionName("GetMatchingRides")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(name: nameof(MatchingFilter.InitialLatitude), In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The latitude of the initial location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.InitialLongitude), In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The longitude of the initial location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.InitialZone), In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The zone of the initial location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.FinalLatitude), In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The latitude of the final location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.FinalLongitude), In = ParameterLocation.Query, Required = true, Type = typeof(float), Description = "The longitude of the final location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.FinalZone), In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The zone of the final location of the ride.")]
        [OpenApiParameter(name: nameof(MatchingFilter.WalkingDistance), In = ParameterLocation.Query, Required = true, Type = typeof(int), Description = "The distance in meters the user asking for a ride is willing to walk.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<Ride>),
            Description = "A list of available rides that match with the search, ordered with the best result at the top.")]
        #endregion
        public async Task<IActionResult> GetMatchingRides(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rides/matching")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(MatchingFilter input)
            {
                try
                {
                    var dto = await rideService.GetMathchingAll(input);
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
                                log.LogError(ex, "Error while looking for rides.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<MatchingFilter>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
