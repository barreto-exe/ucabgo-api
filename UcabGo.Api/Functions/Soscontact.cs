using System;
using System.Collections.Generic;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Soscontact.Dto;
using UcabGo.Core.Data.Soscontact.Inputs;

namespace UcabGo.Api.Functions
{
    public class Soscontact
    {
        private readonly ApiResponse apiResponse;
        private readonly ISoscontactService services;
        public Soscontact(ApiResponse apiResponse, ISoscontactService services)
        {
            this.apiResponse = apiResponse;
            this.services = services;
        }


        #region CreateSoscontact
        [FunctionName("CreateSoscontact")]
        [OpenApiOperation(tags: new[] { "SosContact" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(SoscontactInput),
            Required = true,
            Description = "Create a user's SOS contact.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(SoscontactDto),
            Description = "The info of the SOS contact.")]
        #endregion
        public async Task<IActionResult> CreateSoscontact(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/sos-contacts")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(SoscontactInput input)
            {
                try
                {
                    var dto = await services.Create(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "SOSCONTACT_CREATED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<SoscontactInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetSoscontacts
        [FunctionName("GetSoscontacts")]
        [OpenApiOperation(tags: new[] { "SosContact" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<SoscontactDto>),
            Description = "A list of the data of user's SOS contacts.")]
        #endregion
        public async Task<IActionResult> GetSoscontacts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/sos-contacts")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var list = await services.GetAllDtos(input.Email);
                apiResponse.Data = list;
                apiResponse.Message = "SOSCONTACTS_FOUND";
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region UpdateSoscontacts
        [FunctionName("UpdateSoscontacts")]
        [OpenApiOperation(tags: new[] { "SosContact" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(SoscontactUpdateInput),
            Required = true,
            Description = "Updates an user's SOS contact info.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(SoscontactDto),
            Description = "The updated SOS contact info.")]
        #endregion
        public async Task<IActionResult> UpdateSoscontacts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/sos-contacts")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(SoscontactUpdateInput input)
            {
                try
                {
                    var dto = await services.Update(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "SOSCONTACT_UPDATED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<SoscontactUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region DeleteSoscontact
        [FunctionName("DeleteSoscontact")]
        [OpenApiOperation(tags: new[] { "SosContact" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "id",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the SOS contact to delete.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(SoscontactDto),
            Description = "Info of the deleted SOS contact.")]
        #endregion
        public async Task<IActionResult> DeleteSoscontact(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/sos-contacts/{id:int}")] HttpRequest req, int id, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await services.Delete(input.Email, id);
                    apiResponse.Message = "SOSCONTACT_DELETED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    return new BadRequestObjectResult(apiResponse);
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

    }
}
