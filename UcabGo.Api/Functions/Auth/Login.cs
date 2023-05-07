using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UcabGo.Api.Utils;
using UcabGo.Application.Interfaces;
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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "auth/login")] HttpRequest req,
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
