﻿using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Location.Dtos;
using UcabGo.Core.Data.Location.Inputs;

namespace UcabGo.Api.Functions
{
    public class Location
    {
        private readonly ILocationService locationService;
        private readonly IMapper mapper;
        private readonly ApiResponse apiResponse;
        public Location(ILocationService locationService, IMapper mapper, ApiResponse apiResponse)
        {
            this.locationService = locationService;
            this.mapper = mapper;
            this.apiResponse = apiResponse;
        }

        #region AddHome
        [FunctionName("AddHome")]
        [OpenApiOperation(tags: new[] { "Location" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(HomeInput),
            Required = true,
            Description = "The information of the new location for home.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(LocationDto),
            Description = "The information of the created home.")]
        #endregion
        public async Task<IActionResult> AddHome(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "user/home")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(HomeInput input)
            {
                try
                {
                    var homeInput = mapper.Map<LocationInput>(input);
                    homeInput.Alias = "Casa";
                    homeInput.IsHome = true;

                    var dto = await locationService.Create(homeInput);

                    apiResponse.Message = "HOME_UPDATED";
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
                        default:
                            {
                                log.LogError(ex, "Error while creating location.", input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<HomeInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetHome
        [FunctionName("GetHome")]
        [OpenApiOperation(tags: new[] { "Location" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(LocationDto),
            Description = "The information of the current home.")]
        #endregion
        public async Task<IActionResult> GetHome(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/home")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var dto = await locationService.GetHome(input.Email);
                apiResponse.Message = "HOME_RETRIEVED";
                apiResponse.Data = dto;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetLocations
        [FunctionName("GetLocations")]
        [OpenApiOperation(tags: new[] { "Location" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<LocationDto>),
            Description = "A list with the UCAB and home location of the user.")]
        #endregion
        public async Task<IActionResult> GetLocations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "user/locations")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                var dtos = await locationService.GetLocations(input.Email);
                apiResponse.Message = "LOCATIONS_RETRIEVED";
                apiResponse.Data = dtos;
                return new OkObjectResult(apiResponse);
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }


    }
}