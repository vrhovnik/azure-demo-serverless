using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureServerlessDemo.Functions;

public static class DurableWorkflow
{
    [FunctionName("DurableWorkflow")]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var outputs = new List<string>
        {
            await context.CallActivityAsync<string>("DurableWorkflow_PrepareMessage", "Info"),
            await context.CallActivityAsync<string>("DurableWorkflow_PrepareMessage", "Data"),
            await context.CallActivityAsync<string>("DurableWorkflow_PrepareMessage", "")
        };

        return outputs;
        
    }

    [FunctionName("DurableWorkflow_PrepareMessage")]
    public static async Task<string> PrepareMessage([ActivityTrigger] string message, ILogger log)
    {
        log.LogInformation($"Preparing message {message}.");
        var messageBody = "<h1>Please check your profile data and let us know, if any questions</h1>";
        if (!string.IsNullOrEmpty(message))
        {
            messageBody = message switch
            {
                "data" => "<h1>Please check your portal as we brought to you new features</h1>",
                "info" =>
                    "<h1>You have used your quota to the fullest. Increase to max to get service working.</h1>",
                _ => "<p>Please call us to explain the products</p>"
            };
        }
        return messageBody;
    }

    [FunctionName("DurableWorkflowFunction")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "durablefunc")]
        HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        var instanceId = await starter.StartNewAsync("DurableWorkflow", null);
        log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}