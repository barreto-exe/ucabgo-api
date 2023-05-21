using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
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
        [OpenApiResponseWithoutBody(HttpStatusCode.OK)]
        #endregion
        public async Task<IActionResult> ChangePhone(
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
