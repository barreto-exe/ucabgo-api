using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System;
using System.Collections.Generic;
using System.Web.Http;
using UcabGo.Application.Interfaces;
using UcabGo.Core.Data;
using UcabGo.Core.Data.Chat.Dtos;
using UcabGo.Core.Data.Chat.Input;

namespace UcabGo.Api.Functions
{
    public class Chat
    {
        private readonly ApiResponse apiResponse;
        private readonly IChatService chatService;
        public Chat(ApiResponse apiResponse, IChatService chatService)
        {
            this.apiResponse = apiResponse;
            this.chatService = chatService;
        }


        #region GetMessages
        [FunctionName("GetMessages")]
        [OpenApiOperation(tags: new[] { "Chat" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride's chat.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(IEnumerable<ChatmessageDto>),
            Description = "A list of the messages from the chat.")]
        #endregion
        public async Task<IActionResult> GetMessages(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "rides/{rideId:int}/chat")] HttpRequest req, int rideId, ILogger log)
        {
            async Task<IActionResult> Action(BaseRequest input)
            {
                try
                {
                    var dto = await chatService.GetAllMessages(input.Email, rideId);
                    apiResponse.Data = dto;
                    apiResponse.Message = "MESSAGES_FOUND";
                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "CHAT_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while getting messages.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<BaseRequest>(req, log, apiResponse, Action, isAnonymous: false);
        }



        #region SendMessage
        [FunctionName("SendMessage")]
        [OpenApiOperation(tags: new[] { "Chat" })]
        [OpenApiSecurity("bearerAuth", SecuritySchemeType.Http, Scheme = OpenApiSecuritySchemeType.Bearer, BearerFormat = "JWT")]
        [OpenApiParameter(
            name: "rideId",
            In = ParameterLocation.Path,
            Required = true,
            Type = typeof(int),
            Description = "The ID of the ride's chat.")]
        [OpenApiRequestBody(
            contentType: "application/json",
            bodyType: typeof(ChatmessageInput),
            Required = true,
            Description = "The message that's goind to be sent.")]
        [OpenApiResponseWithBody(
            statusCode: HttpStatusCode.OK,
            contentType: "application/json",
            bodyType: typeof(ChatmessageDto),
            Description = "The message sent.")]
        #endregion
        public async Task<IActionResult> SendMessage(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "rides/{rideId:int}/chat")] HttpRequest req,
            [SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages,
            int rideId, ILogger log)
        {
            async Task<IActionResult> Action(ChatmessageInput input)
            {
                try
                {
                    input.Ride = rideId;
                    var dto = await chatService.SendMessage(input);
                    apiResponse.Data = dto;
                    apiResponse.Message = "MESSAGE_SENT";

                    await signalRMessages.Send("ReceiveMessage", dto.UsersToMessage, new object[] { rideId });

                    return new OkObjectResult(apiResponse);
                }
                catch (Exception ex)
                {
                    apiResponse.Message = ex.Message;
                    switch (ex.Message)
                    {
                        case "CHAT_NOT_FOUND":
                            return new NotFoundObjectResult(apiResponse);
                        default:
                            {
                                log.LogError(ex, "Error while sending message.\n" +  ex.Message + "\n" + ex.StackTrace, input);
                                return new InternalServerErrorResult();
                            }
                    }
                }
            }

            return await RequestHandler.Handle<ChatmessageInput>(req, log, apiResponse, Action, isAnonymous: false);
        }
    }
}
