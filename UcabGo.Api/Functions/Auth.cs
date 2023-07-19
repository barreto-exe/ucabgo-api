using System;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Application.Services;
using UcabGo.Application.Utils;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Auth.Dto;
using UcabGo.Core.Data.Auth.Filters;
using UcabGo.Core.Data.Auth.Inputs;
using UcabGo.Core.Data.Ride.Filters;

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
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "USER_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while changing password.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
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
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;

                    switch (ex.Message)
                    {
                        case "WRONG_CREDENTIALS":
                            return new BadRequestObjectResult(apiResponse);
                        case "USER_NOT_VALIDATED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while logging in.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
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
                input.ValidationUrl = Environment.GetEnvironmentVariable("ApiValidationUrl");

                try
                {
                    await authService.Register(input);
                    apiResponse.Message = "USER_REGISTERED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "USER_ALREADY_EXISTS":
                            return new BadRequestObjectResult(apiResponse);
                        case "REGISTER_FIELD_LENGTH":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while registering user.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<RegisterInput>(req, log, apiResponse, Action, isAnonymous: true);
        }


        #region ValidateEmail
        [FunctionName("ValidateEmail")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(ValidationFilter.ValidationEmail),
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(string),
            Description = "The email of the user")]
        [OpenApiParameter(
            name: nameof(ValidationFilter.ValidationGuid),
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(string),
            Description = "The guid that was sent to the user's email")]
        #endregion
        public async Task<IActionResult> ValidateEmail(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/validate")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(ValidationFilter input)
            {
                try
                {
                    var html = await authService.ValidateUser(input.ValidationEmail, input.ValidationGuid);
                    return new ContentResult
                    {
                        Content = html,
                        ContentType = "text/html",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
                catch(Exception ex)
                {
                    return new NotFoundResult();
                }
            }

            return await RequestHandler.Handle<ValidationFilter>(req, log, apiResponse, Action, isAnonymous: true);
        }


        #region RequestNewGuid
        [FunctionName("RequestNewGuid")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(ValidationFilter.ValidationEmail),
            In = ParameterLocation.Query,
            Required = true,
            Type = typeof(string),
            Description = "The email of the user.")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(BaseRequest),
            Required = true,
            Description = "RequestBodyDescription")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The new verification sent successfully.")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.NotFound, Description = "User not found.")]
        #endregion
        public async Task<IActionResult> RequestNewGuid(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "auth/validate/new")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(ValidationFilter input)
            {
                input.ValidationUrl = Environment.GetEnvironmentVariable("ApiValidationUrl");

                try
                {
                    await authService.RequestNewValidationGuid(input.ValidationEmail, input.ValidationUrl);
                    apiResponse.Message = "NEW_GUID_SENT";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "NOT_FOUND":
                            return new NotFoundResult();
                        default:
                            {
                                log.LogError(ex, "Error while requesting new validation guid.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<ValidationFilter>(req, log, apiResponse, Action, isAnonymous: true);
        }

    }
}
