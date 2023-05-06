using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using UcabGo.Api.Utils;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Api.Functions.Auth
{
    public class Register
    {
        private readonly IAuthService authService;
        public Register(IAuthService authService)
        {
            this.authService = authService;
        }

        [FunctionName("Register")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth/register")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(RegisterInput input)
            {
                LoginDto data = null;

                try
                {
                    data = await authService.Register(input);
                }
                catch (UserExistsException)
                {
                    return new BadRequestObjectResult(IAuthService.ErrorCodeBuilder("USER_ALREADY_EXISTS"));
                }

                return new OkObjectResult(data);
            }

            return await RequestHandler.Handle<RegisterInput>(req, log, Action, isAnonymous: true);
        }
    }
}
