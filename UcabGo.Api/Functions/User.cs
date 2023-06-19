using System;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.User.Inputs;

namespace UcabGo.Api.Functions
{
    public class User
    {
        private readonly ApiResponse apiResponse;
        private readonly IUserService userService;
        public User(ApiResponse apiResponse, IUserService userService)
        {
            this.apiResponse = apiResponse;
            this.userService = userService;
        }
        #region ChangePhone
        [FunctionName("ChangePhone")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(PhoneInput),
            Required = true,
            Description = "Change users phone.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(UserDto),
            Description = "The data of the user updated.")]
        #endregion
        public async Task<IActionResult> ChangePhone(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/phone")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(PhoneInput input)
            {
                var dto = await userService.UpdatePhone(input);
                apiResponse.Message = "PHONE_UPDATED";
                apiResponse.Data = dto;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<PhoneInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region ChangeWalkingDistance
        [FunctionName("ChangeWalkingDistance")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(WalkingInput),
            Required = true,
            Description = "The amount of meters a ride soliciter is willing to walk.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(UserDto),
            Description = "The data of the user updated.")]
        #endregion
        public async Task<IActionResult> ChangeWalkingDistance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/walking-distance")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(WalkingInput input)
            {
                try
                {
                    var dto = await userService.UpdateWalkingDistance(input);
                    apiResponse.Message = "WALKING_DISTANCE_UPDATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "LIMIT_REACHED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex.Message);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<WalkingInput>(req, log, apiResponse, Action, isAnonymous: false);
        }

    }
}
