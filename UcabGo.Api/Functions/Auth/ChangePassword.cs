using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Api.Functions.Auth
{
    public class ChangePassword
    {
        private readonly ApiResponse apiResponse;
        private readonly IAuthService authService;
        public ChangePassword(ApiResponse apiResponse, IAuthService authService)
        {
            this.apiResponse = apiResponse;
            this.authService = authService;
        }

        [FunctionName("ChangePassword")]
        [OpenApiOperation(tags: new[] { "Auth" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ChangePasswordInput), Required = true, Description = "Change the password of a user.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "auth/changepassword")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(ChangePasswordInput input)
            {
                try
                {
                    await authService.ChangePassword(input);
                    apiResponse.Message = "PASSWORD_CHANGED";
                    return new OkObjectResult(apiResponse);
                }
                catch (UserNotFoundException)
                {
                    apiResponse.Message = "USER_NOT_FOUND";
                    return new NotFoundObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<ChangePasswordInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
