using AutoMapper;
using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;

namespace UcabGo.Api.Functions
{
    public class Destination
    {
        private readonly ApiResponse apiResponse;
        private readonly ILocationService services;
        public Destination(ApiResponse apiResponse, ILocationService services)
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
            bodyType: typeof(LocationInput),
            Required = true,
            Description = "Create a user's destination.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(LocationDto),
            Description = "The info of the user's destination.")]
        #endregion
        public async Task<IActionResult> CreateDestination(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/destinations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(LocationInput input)
            {
                try
                {
                    var dto = await services.Create(input);
                    apiResponse.Message = "LOCATION_CREATED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "UCAB_LOCATION_ALREADY_CREATED":
                            return new BadRequestObjectResult(apiResponse);
                        case "LOCATION_EMPTY_DETAILS":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while creating destination.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<LocationInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetDestination
        [FunctionName("GetDestination")]
        [OpenApiOperation(tags: new[] { "Destination" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<LocationDto>),
            Description = "A list of the data of user's destinations.")]
        #endregion
        public async Task<IActionResult> GetDestinations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/destinations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var list = await services.GetAllDtos(input.Email);
                apiResponse.Data = list;
                apiResponse.Message = "LOCATIONS_FOUND";
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
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
            bodyType: typeof(LocationDto),
            Description = "Info of the deleted destination.")]
        #endregion
        public async Task<IActionResult> DeleteDestination(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "user/destinations/{id:int}")] HttpRequest req, int id, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await services.Delete(id, input.Email);
                    apiResponse.Message = "LOCATION_DELETED";
                    apiResponse.Data = dto;
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "UCAB_LOCATION_IS_READONLY":
                            return new BadRequestObjectResult(apiResponse);
                        case "HOME_LOCATION_IS_READONLY":
                            return new BadRequestObjectResult(apiResponse);
                        case "LOCATION_NOT_FOUND":
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
