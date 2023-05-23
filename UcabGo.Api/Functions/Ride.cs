using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Ride.Dtos;
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
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<RideInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region UpdateRide
        [FunctionName("UpdateRide")]
        [OpenApiOperation(tags: new[] { "Ride" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(RideUpdateInput),
            Required = true,
            Description = "The new data for the ride.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(RideDto),
            Description = "The updated data of the ride.")]
        #endregion
        public async Task<IActionResult> UpdateRide(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "rides")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RideUpdateInput input)
            {
                try
                {
                    var dto = await rideService.Update(input);
                    apiResponse.Message = "RIDE_UPDATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<RideUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

    }
}
