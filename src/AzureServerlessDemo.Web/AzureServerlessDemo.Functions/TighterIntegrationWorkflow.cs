using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Azure.Messaging.EventGrid;
using AzureServerlessDemo.Core;
using AzureServerlessDemo.Functions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace AzureServerlessDemo.Functions;

public static class TighterIntegrationWorkflow
{
    [FunctionName("TighterIntegrationWorkflow")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sendcontactstighter/{message}")]
        HttpRequest req,
        string message,
        [Queue("serverlessemails")] IAsyncCollector<CloudQueueMessage> emailCollector,
        [Table("serverlesslogs")] IAsyncCollector<LogModel> tableCollector,
        [EventGrid(TopicEndpointUri = "EventGridNotificationEndpoint", TopicKeySetting = "EventGridKey")]
        IAsyncCollector<EventGridEvent> eventCollector,
        ILogger log)
    {
        log.LogInformation("Tighter integration workflow to prepare message and then do additional executions");

        try
        {
            var stopwatch = Stopwatch.StartNew();

            //send emails
            var data = JsonConvert.SerializeObject(new EmailModel { Subject = "Info about data usage - tighter" });
            await emailCollector.AddAsync(new CloudQueueMessage(data));

            log.LogInformation("Addding data to Azure Tables for log purposes");

            var logItem = new LogModel
            {
                Text = $"Calling tighter integration workflow with {message}",
                CalledFromMethod = "TighterIntegrationWorkflow",
                LoggedDate = DateTime.Now
            };

            await tableCollector.AddAsync(logItem); //add to logs for future review

            //add to all subscribers to seen information via action
            await eventCollector.AddAsync(new EventGridEvent("Tighter integration", "TigtherIntegration",
                "1.0.0", logItem));

            stopwatch.Stop();

            var logMessage = $"Sending email to clients and it took {stopwatch.ElapsedMilliseconds}";
            log.LogInformation(logMessage);

            return new OkObjectResult(logMessage);
        }
        catch (Exception e)
        {
            log.LogError(e.Message);
            return new BadRequestResult();
        }
    }
}