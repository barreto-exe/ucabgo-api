using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UcabGo.Api.Utils;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Api.Functions.Auth
{
    public class Login
    {
        private readonly IAuthService authService;
        private ApiResponse apiResponse;
        public Login(IAuthService authService, ApiResponse apiResponse)
        {
            this.authService = authService;
            this.apiResponse = apiResponse;
        }

        [FunctionName("Login")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Login" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(LoginInput), Required = true, Description = "The user login details")]
        [OpenApiResponseWithBody(statusCode: System.Net.HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(LoginDto), Description = "The login details of the registered user")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/login")] HttpRequest req,
            ILogger log)
        {
            async Task<IActionResult> Action(LoginInput input)
            {
                try
                {
                    apiResponse.Message = "USER_LOGGED_IN";
                    apiResponse.Data = await authService.Login(input);
                    return new OkObjectResult(apiResponse);
                }
                catch (UserNotFoundException)
                {
                    apiResponse.Message = "WRONG_CREDENTIALS";
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<LoginInput>(req, log, apiResponse, Action, isAnonymous: true);
        }
    }
}
