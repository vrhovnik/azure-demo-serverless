// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName=HandleEventGridMessages

using System;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;

namespace AzureServerlessDemo.Functions;

public static class HandleEventGridMessages
{
    [FunctionName("HandleEventGridMessages")]
    public static async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent,
        [SignalR(HubName = "serverlessSample")]
        IAsyncCollector<SignalRMessage> signalRMessages,
        ILogger logger)
    {
        var message = eventGridEvent.Data.ToString();
        logger.LogInformation(message);
        logger.LogInformation("Event received {type} {subject}", eventGridEvent.EventType, eventGridEvent.Subject);
        await signalRMessages.AddAsync(
            new SignalRMessage
            {
                Target = "newMessage",
                Arguments = new[] { message }
            });
        logger.LogInformation($"Information was sent to listeners at {DateTime.Now}");
    }

    [FunctionName("negotiate")]
    public static SignalRConnectionInfo Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous)]
        HttpRequest req,
        [SignalRConnectionInfo(HubName = "serverlessSample")]
        SignalRConnectionInfo connectionInfo) => connectionInfo;
}