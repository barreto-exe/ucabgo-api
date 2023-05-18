using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Api.Utils;
using UcabGo.Application.Services;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Application.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;

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
        [OpenApiOperation(tags: new[] { "ChangePassword" })]
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
                    return new OkResult();
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
