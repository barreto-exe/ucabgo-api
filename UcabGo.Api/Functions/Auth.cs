using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Exceptions;
using UcabGo.Core.Data.Auth.Inputs;

namespace UcabGo.Api.Functions
{
    public class Auth
    {
        private readonly ApiResponse apiResponse;
        private readonly IAuthService authService;
        public Auth(ApiResponse apiResponse, IAuthService authService)
        {
            this.apiResponse = apiResponse;
            this.authService = authService;
        }

        #region ChangePassword
        [FunctionName("ChangePassword")]
        [OpenApiOperation(tags: new[] { "Auth" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(ChangePasswordInput), 
            Required = true, 
            Description = "Change the password of a user.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK)]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.BadRequest)]
        #endregion
        public async Task<IActionResult> ChangePassword(
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

        #region Login
        [FunctionName("Login")]
        [OpenApiOperation(tags: new[] { "Auth" })]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(LoginInput), 
            Required = true, 
            Description = "The user login details")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK, 
            contentType: "application/json", 
            bodyType: typeof(LoginDto), 
            Description = "The login details of the registered user")]
        #endregion
        public async Task<IActionResult> Login(
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

        #region Register
        [FunctionName("Register")]
        [OpenApiOperation(tags: new[] { "Auth" })]
        [OpenApiRequestBody(
            contentType: "application/json", 
            bodyType: typeof(RegisterInput), 
            Required = true, 
            Description = "The user registration details")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK, 
            contentType: "application/json", 
            bodyType: typeof(LoginDto), 
            Description = "The login details of the registered user")]
        #endregion
        public async Task<IActionResult> Register(
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
