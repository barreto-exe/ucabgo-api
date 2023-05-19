using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Api.Functions.Auth
{
    public class Register
    {
        private readonly IAuthService authService;
        private ApiResponse apiResponse;
        public Register(IAuthService authService, ApiResponse apiResponse)
        {
            this.authService = authService;
            this.apiResponse = apiResponse;
        }

        [FunctionName("Register")]
        [OpenApiOperation(tags: new[] { "Auth" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(RegisterInput), Required = true, Description = "The user registration details")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginDto), Description = "The login details of the registered user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RegisterInput input)
            {
                try
                {
                    apiResponse.Message = "USER_REGISTERED";
                    apiResponse.Data = await authService.Register(input);
                    return new OkObjectResult(apiResponse);
                }
                catch (UserExistsException)
                {
                    apiResponse.Message = "USER_ALREADY_EXISTS";
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<RegisterInput>(req, log, apiResponse, Action, isAnonymous: true);
        }
    }
}
