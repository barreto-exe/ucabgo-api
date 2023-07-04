using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data.Evaluation.Dtos;
using UcabGo.Core.Data.Evaluation.Filter;
using UcabGo.Core.Data.Evaluation.Inputs;

namespace UcabGo.Api.Functions
{
    public class Evaluation
    {
        private readonly ApiResponse apiResponse;
        private readonly IEvaluationService evaluationService;
        public Evaluation(ApiResponse apiResponse, IEvaluationService evaluationService)
        {
            this.apiResponse = apiResponse;
            this.evaluationService = evaluationService;
        }


        #region CreateEvaluation
        [FunctionName("CreateEvaluation")]
        [OpenApiOperation(tags: new[] { "Stars" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(EvaluationInput),
            Required = true,
            Description = "The information of the evaluation and the stars given. The range of stars must be between 1 and 5. The EvaluatorType must 'P' for passenger or 'D' for Driver.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(EvaluationDto),
            Description = "The information of the loaded evaluation")]
        #endregion
        public async Task<IActionResult> CreateEvaluation(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "stars")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(EvaluationInput input)
            {
                try
                {
                    var dto = await evaluationService.AddEvaluation(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "EVALUATION_LOADED";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "RIDE_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "EVALUATOR_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "EVALUATED_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        case "INVALID_STARS":
                            return new BadRequestObjectResult(apiResponse);
                        case "EVALUATION_EXISTS":
                            return new BadRequestObjectResult(apiResponse);
                        case "EVALUATING_YOURSELF":
                            return new BadRequestObjectResult(apiResponse);
                        case "WRONG_TYPE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while creating an evalution.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }
            return await RequestHandler.Handle<EvaluationInput>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetRecievedEvaluations
        [FunctionName("GetRecievedEvaluations")]
        [OpenApiOperation(tags: new[] { "Stars" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(EvaluationFilter.Type),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(string),
            Description = "ALL = All recieved evaluations. | \"P\": Evaluations recieved as passenger. | \"D\": Evaluations recieved as driver.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<EvaluationDto>),
            Description = "A list of the evaluations recieved.")]
        #endregion
        public async Task<IActionResult> GetRecievedEvaluations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stars/recieved")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(EvaluationFilter input)
            {
                try
                {
                    var dto = await evaluationService.GetRecievedEvaluations(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "EVALUATIONS_FOUND";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "WRONG_TYPE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting recieved evaluations.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<EvaluationFilter>(req, log, apiResponse, Action, isAnonymous: true);
        }


        #region GetGivenEvaluations
        [FunctionName("GetGivenEvaluations")]
        [OpenApiOperation(tags: new[] { "Stars" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(EvaluationFilter.Type),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(string),
            Description = "ALL = All given evaluations. | \"P\": Evaluations given as passenger. | \"D\": Evaluations given as driver.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<EvaluationDto>),
            Description = "A list of the evaluations given.")]
        #endregion
        public async Task<IActionResult> GetGivenEvaluations(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stars/given")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(EvaluationFilter input)
            {
                try
                {
                    var dto = await evaluationService.GetGivenEvaluations(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "EVALUATIONS_FOUND";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "WRONG_TYPE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting recieved evaluations.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<EvaluationFilter>(req, log, apiResponse, Action, isAnonymous: false);
        }


        #region GetAverageStars
        [FunctionName("GetAverageStars")]
        [OpenApiOperation(tags: new[] { "Stars" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: nameof(EvaluationFilter.Type),
            In = ParameterLocation.Query,
            Required = false,
            Type = typeof(string),
            Description = "ALL = All recieved evaluations. | \"P\": Evaluations recieved as passenger. | \"D\": Evaluations recieved as driver.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(float),
            Description = "The average of stars between 1 and 5.")]
        #endregion
        public async Task<IActionResult> GetAverageStars(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "stars/average")] HttpRequest req, ILogger log)
        {
            async Task<IActionResult> Action(EvaluationFilter input)
            {
                try
                {
                    var dto = await evaluationService.GetRecievedStarsAverage(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "EVALUATIONS_FOUND";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "WRONG_TYPE":
                            return new BadRequestObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting recieved evaluations average.\n" + ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<EvaluationFilter>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
