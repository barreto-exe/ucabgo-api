using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;
using UcabGo.Api.Utils;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.User.Inputs;

namespace UcabGo.Api.Functions.User
{
    public class ChangePhone
    {
        private readonly ApiResponse apiResponse;
        private readonly IUserService userService;
        public ChangePhone(ApiResponse apiResponse, IUserService userService)
        {
            this.apiResponse = apiResponse;
            this.userService = userService;
        }

        [FunctionName("ChangePhone")]
        [OpenApiOperation(tags: new[] { "ChangePhone" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(PhoneInput), Required = true, Description = "Change users phone.")]
        [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/phone")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(PhoneInput input)
            {
                await userService.UpdatePhone(input);
                apiResponse.Message = "PHONE_UPDATED";
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<PhoneInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }

}
