using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Destination.Dtos;
using UcabGo.Core.Data.Destination.Inputs;

namespace UcabGo.Api.Functions
{
    public class Destination
    {
        private readonly ApiResponse apiResponse;
        private readonly IDestinationService services;
        public Destination(ApiResponse apiResponse, IDestinationService services)
        {
            this.apiResponse = apiResponse;
            this.services = services;
        }


        #region CreateDestination
        [FunctionName("CreateDestination")]
        [OpenApiOperation(tags: new[] { "Destination" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(DestinationInput),
            Required = true,
            Description = "Create a user's destination.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(DestinationDto),
            Description = "The info of the user's destination.")]
        #endregion
        public async Task<IActionResult> CreateDestination(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/destinations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(DestinationInput input)
            {
                try
                {
                    var dto = await services.Create(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "DESTINATION_CREATED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "UCAB_DESTINATION_ALREADY_CREATED":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while creating destination.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<DestinationInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetDestination
        [FunctionName("GetDestination")]
        [OpenApiOperation(tags: new[] { "Destination" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<DestinationDto>),
            Description = "A list of the data of user's destinations.")]
        #endregion
        public async Task<IActionResult> GetDestinations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/destinations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var list = await services.GetAllDtos(input.Email);
                apiResponse.Data = list;
                apiResponse.Message = "DESTINATIONS_FOUND";
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region UpdateDestination
        [FunctionName("UpdateDestination")]
        [OpenApiOperation(tags: new[] { "Destination" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(DestinationUpdateInput),
            Required = true,
            Description = "Updates an user's destination info.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(DestinationDto),
            Description = "The updated destination info.")]
        #endregion
        public async Task<IActionResult> UpdateDestination(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "user/destinations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(DestinationUpdateInput input)
            {
                try
                {
                    var dto = await services.Update(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "DESTINATION_UPDATED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "UCAB_DESTINATION_IS_READONLY":
                            return new BadRequestObjectResult(apiResponse);
                        case "DESTINATION_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "DESTINATION_NULL_FIELD":
                            return new BadRequestObjectResult(apiResponse);
                        case "DESTINATION_FIELD_LENGTH":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while updating destination.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<DestinationUpdateInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region DeleteDestination
        [FunctionName("DeleteDestination")]
        [OpenApiOperation(tags: new[] { "Destination" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "id",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the destination to delete.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(DestinationDto),
            Description = "Info of the deleted destination.")]
        #endregion
        public async Task<IActionResult> DeleteDestination(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/destinations/{id:int}")] HttpRequest req, int id, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await services.Delete(input.Email, id);
                    apiResponse.Message = "DESTINATION_DELETED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "UCAB_DESTINATION_IS_READONLY":
                            return new BadRequestObjectResult(apiResponse);
                        case "DESTINATION_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while deleting destination with id {ID}.", id);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }

    }
}
