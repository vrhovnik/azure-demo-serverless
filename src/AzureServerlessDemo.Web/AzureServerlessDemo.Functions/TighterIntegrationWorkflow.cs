using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AzureServerlessDemo.Core;
using AzureServerlessDemo.Functions.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
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
        [Table("serverlesslogs")]IAsyncCollector<LogModel> tableCollector,
        ILogger log)
    {
        log.LogInformation("Tighter integration workflow to prepare message and then do additional executions");

        try
        {
            var stopwatch = Stopwatch.StartNew();

            var data = JsonConvert.SerializeObject(new EmailModel { Subject = "Info about data usage - tighter" });
            await emailCollector.AddAsync(new CloudQueueMessage(data));

            log.LogInformation("Addding data to Azure Tables for log purposes");
            
            var logItem = new LogModel
            {
                PartitionKey = "logs",
                RowKey = Guid.NewGuid().ToString(),
                Text = $"Calling tighter integration workflow with {message}",
                CalledFromMethod = "TighterIntegrationWorkflow",
                LoggedDate = DateTime.Now
            };
            
            await tableCollector.AddAsync(logItem);
            
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