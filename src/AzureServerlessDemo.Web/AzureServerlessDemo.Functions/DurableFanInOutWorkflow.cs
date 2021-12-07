using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureServerlessDemo.Functions;

public static class DurableFanInOutWorkflow
{
    [FunctionName("DurableFanInOutWorkflow")]
    public static async Task<IActionResult> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "durablefuncfaninout/{blobDirectory}")]
        HttpRequestMessage req,
        string blobDirectory,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        log.LogInformation("Calling fan in and out");
        var result = await starter.StartNewAsync("DoCalculationsFun", blobDirectory);
        return new OkObjectResult("All files sizes: " + result);
    }

    [FunctionName("DoCalculationsFun")]
    public static async Task<int> DoCalculations([ActivityTrigger] IDurableOrchestrationContext activityContext, CancellationToken ctk)
    {
        var directoryName = activityContext.GetInput<string>();
        //get all blobs inside and do calculations
        var parralelTasks = new List<Task<int>>();
        var blobNames = await activityContext.CallActivityAsync<string[]>("GetBlobNames",directoryName);

        foreach (var currentBlobName in blobNames)
        {
            var task = activityContext.CallActivityAsync<int>("CalculateSize", currentBlobName);
            parralelTasks.Add(task);
        }

        await Task.WhenAll(parralelTasks);
        return parralelTasks.Sum(currentBlobSize => currentBlobSize.Result);
    }
    
    [FunctionName("GetBlobNames")]
    public static string[] GetBlobNames([ActivityTrigger]string blobDirName, ILogger log)
    {
        log.LogInformation($"Getting blob names from container {blobDirName}");
        //TODO: read blob container and get names to calculate sizes
        return new[] { ""};
    }
    
    [FunctionName("CalculateSize")]
    public static int CalculateSize([ActivityTrigger]string blobName, ILogger log)
    {
        log.LogInformation($"Getting blob {blobName} and reading lines of files");
        //TODO: read the blob and return lines of kode
        return 0;
    }
}