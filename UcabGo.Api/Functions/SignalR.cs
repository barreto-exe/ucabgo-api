using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using UcabGo.Core.Data;

namespace UcabGo.Api.Functions
{
    public class SignalR
    {
        [FunctionName("Negotiate")]
        public async Task<SignalRConnectionInfo> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req,
            IBinder binder,
            ILogger log)
        {
            SignalRConnectionInfo connectionInfo = null;

            async Task<IActionResult> Action(BaseRequest input)
            {
                connectionInfo = await binder.BindAsync<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute
                {
                    HubName = req.Headers["HubName"],
                    UserId = input.Email,
                });

                return null;
            }

            await RequestHandler.Handle<BaseRequest>(req, log, null, Action, isAnonymous: false);

            return connectionInfo;
        }
    }
}