using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using UcabGo.Api.Utils.JWT;
using UcabGo.Application.Utils;
using UcabGo.Core.Data;

namespace UcabGo.Api.Utils
{
    public static class RequestHandler
    {
        public static async Task<IActionResult> Handle<TInput>(HttpRequest req, ILogger log, ApiResponse responseWrapper, Func<TInput, Task<IActionResult>> function, bool isAnonymous = false, BodyTypeEnum bodyType = BodyTypeEnum.Json)
            where TInput : BaseRequest, new()
        {
            //Validating jwt
            var jwt = new JWTValidator(req);
            if (!jwt.IsValid && !isAnonymous)
            {
                return new UnauthorizedResult();
            }

            TInput input;
            try
            {
                // A base request has no aditional parameters 
                if (typeof(TInput) == typeof(BaseRequest) || req.Method.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                {
                    input = new();
                }
                else if (req.Method.Equals("get", StringComparison.InvariantCultureIgnoreCase))
                {
                    input = req.Query.ToObject<TInput>();
                }
                else
                {
                    switch (bodyType)
                    {
                        case BodyTypeEnum.Json:
                            {
                                input = await req.Body.ToObjectAsync<TInput>();

                                if (input == null)
                                {
                                    if (req.Method.Equals("delete", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        input = new();
                                    }
                                    else
                                    {
                                        throw new Exception("Request body is empty.");
                                    }
                                }
                                break;
                            }
                        case BodyTypeEnum.Formdata:
                            {
                                input = req.Form.ToObject<TInput>();

                                //We obtain the files uploaded via Form, then we need to manually set the file property
                                var files = req.Form.Files.ToList();
                                foreach (var file in files)
                                {
                                    var settings =
                                        BindingFlags.Instance |
                                        BindingFlags.Public |
                                        BindingFlags.SetProperty |
                                        BindingFlags.IgnoreCase;

                                    //Try to match the file name to a property in the input object
                                    var prop = input.GetType().GetProperty(file.Name, settings);
                                    if (prop != null && prop.CanWrite)
                                    {
                                        prop.SetValue(input, file);
                                    }
                                }

                                break;
                            }
                        default:
                            throw new Exception("Body type not supported.");
                    }
                }

                input.Email = jwt.Claims?.FirstOrDefault(x => x.Type == "email")?.Value;

                if (!input.IsValid(out List<ValidationResult> validationResults))
                {
                    if (responseWrapper == null) return new BadRequestObjectResult(validationResults);

                    responseWrapper.Message = "INVALID_INPUT";
                    responseWrapper.Data = validationResults;
                    return new BadRequestObjectResult(responseWrapper);
                }

                return await function.Invoke(input);
            }
            catch (Exception ex)
            {
                log.LogError(ex, ex.Message);
                
                if (responseWrapper == null) return new BadRequestObjectResult(ex.Message);

                responseWrapper.Message = "INTERNAL_SERVER_ERROR";
                responseWrapper.Data = "An error has ocurred internally: " + ex.Message;
                return new BadRequestObjectResult(responseWrapper);
            }
        }

        public static async Task Send(this IAsyncCollector<SignalRMessage> signalRMessages, string target, IEnumerable<string> usersToMessage, object[] args)
        {
            var tasks = new List<Task>();
            foreach (var user in usersToMessage.Distinct())
            {
                tasks.Add(signalRMessages.AddAsync(new SignalRMessage
                {
                    Target = target,
                    UserId = user,
                    Arguments = args
                }));
            }

            await Task.WhenAll(tasks);
        }

        public static async Task Send(this IAsyncCollector<SignalRMessage> signalRMessages, string target, object[] args)
        {
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = target,
                Arguments = args
            });
        }

        public static async Task Send(this IAsyncCollector<SignalRMessage> signalRMessages, string target)
        {
            await signalRMessages.AddAsync(new SignalRMessage
            {
                Target = target,
            });
        }

        public enum BodyTypeEnum
        {
            Json,
            Formdata,
        }
    }
}
