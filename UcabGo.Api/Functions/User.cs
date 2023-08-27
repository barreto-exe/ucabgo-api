using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.User.Dto;
using UcabGo.Core.Data.User.Inputs;
using static UcabGo.Api.Utils.RequestHandler;

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
        #region ChangePersonalInfo
        [FunctionName("ChangePersonalInfo")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(UserUpdateInput),
            Required = true,
            Description = "Change users personal information.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(UserDto),
            Description = "The data of the user updated.")]
        #endregion
        public async Task<IActionResult> ChangePersonalInfo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(UserUpdateInput input)
            {
                var dto = await userService.UpdatePersonalInfo(input);
                apiResponse.Message = "USER_UPDATED";
                apiResponse.Data = dto;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<UserUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region ChangeWalkingDistance
        [FunctionName("ChangeWalkingDistance")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(WalkingInput),
            Required = true,
            Description = "The amount of meters a ride soliciter is willing to walk.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(UserDto),
            Description = "The data of the user updated.")]
        #endregion
        public async Task<IActionResult> ChangeWalkingDistance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/walking-distance")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(WalkingInput input)
            {
                try
                {
                    var dto = await userService.UpdateWalkingDistance(input);
                    apiResponse.Message = "WALKING_DISTANCE_UPDATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "LIMIT_REACHED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while changing walking distance.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<WalkingInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region ChangeProfilePicture
        [FunctionName("ChangeProfilePicture")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        //[OpenApiRequestBody(
        //    contentType: "multipart/form-data",
        //    bodyType: typeof(ProfilePictureInput),
        //    Required = true,
        //    Description = "The file for the profile picture. Must be an jpg or png.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(UserDto),
            Description = "The data of the updated user.")]
        #endregion
        public async Task<IActionResult> ChangeProfilePicture(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/picture")] HttpRequest req, 
            IBinder binder,
            ILogger log)
        {
            async Task<IActionResult> Action(ProfilePictureInput input)
            {
                try
                {
                    //Validate if the file is an image
                    var file = input.Picture.FileName;
                    var extension = file[(file.LastIndexOf('.') + 1)..];
                    if (extension != "jpg" && extension != "jpeg" && extension != "png")
                    {
                        apiResponse.Message = "INVALID_FILE";
                        return new BadRequestObjectResult(apiResponse);
                    }

                    //File path must be the user of the email with a timestamp
                    string filePath = 
                        "pictures/" +
                        string.Concat(input.Email.AsSpan(0, input.Email.IndexOf('@')), DateTime.Now.ToString("yyyyMMddHHmmss"), ".", extension);

                    //Upload the file to the blob storage
                    var url = "";
                    using (var streamReader = new StreamReader(input.Picture.OpenReadStream()))
                    {
                        var outputBlob = await binder.BindAsync<BlobClient>(new BlobAttribute(filePath, FileAccess.Write));
                        await outputBlob.UploadAsync(streamReader.BaseStream);
                        url = outputBlob.Uri.ToString();
                    }

                    log.LogError("Uploaded image URL: " + url);

                    //Update the user with the new profile picture
                    var dto = await userService.UpdateProfilePicture(input.Email, url, log);
                    apiResponse.Message = "PROFILE_PICTURE_UPDATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        default:
                            {
                                log.LogError(ex, "Error while changing profile picture.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<ProfilePictureInput>(req, log, apiResponse, Action, isAnonymous: false, BodyTypeEnum.Formdata);
        }


        #region UploadImage
        [FunctionName("UploadImage")]
        [OpenApiOperation(tags: new[] { "User" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        #endregion
        public async Task<IActionResult> UploadImage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "upload")] HttpRequest req,
            IBinder binder,
            ILogger log)
        {
            async Task<IActionResult> Action(ProfilePictureInput input)
            {
                try
                {
                    //File path must be the user of the email with a timestamp
                    string filePath = "pictures/" + Guid.NewGuid().ToString();

                    //Upload the file to the blob storage
                    var url = "";
                    using (var streamReader = new StreamReader(input.Picture.OpenReadStream()))
                    {
                        var outputBlob = await binder.BindAsync<BlobClient>(new BlobAttribute(filePath, FileAccess.Write));
                        await outputBlob.UploadAsync(streamReader.BaseStream);
                        url = outputBlob.Uri.ToString();
                    }

                    log.LogError("Uploaded image URL: " + url);

                    //Update the user with the new profile picture
                    return new OkObjectResult(url);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        default:
                            {
                                log.LogError(ex, "Error while changing profile picture.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<ProfilePictureInput>(req, log, apiResponse, Action, isAnonymous: true, BodyTypeEnum.Formdata);
        }

    }
}
