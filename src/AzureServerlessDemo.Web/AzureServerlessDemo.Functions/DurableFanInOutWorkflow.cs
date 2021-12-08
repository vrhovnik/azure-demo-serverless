using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AzureServerlessDemo.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace AzureServerlessDemo.Functions;

public class DurableFanInOutWorkflow
{
    private string currentBlobDirectory = "durableblobs";
    private readonly string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

    [FunctionName("DurableFanInOutWorkflow")]
    public async Task<IActionResult> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "durablefuncfaninout/{blobDirectory}")]
        HttpRequestMessage req,
        string blobDirectory,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        log.LogInformation("Calling fan in and out");
        currentBlobDirectory = blobDirectory;
        var result = await starter.StartNewAsync("DoCalculationsFun", blobDirectory);
        return new OkObjectResult("All files sizes: " + result);
    }

    [FunctionName("DoCalculationsFun")]
    public async Task<int> DoCalculations([ActivityTrigger] IDurableOrchestrationContext activityContext, 
        CancellationToken ctk)
    {
        var directoryName = activityContext.GetInput<string>();
        
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
    public async Task<string[]> GetBlobNames([ActivityTrigger]string blobDirName, ILogger log)
    {
        log.LogInformation($"Getting blob names from container {blobDirName}");
        
        var azureStorageHelper = new AzureStorageHelper(storageConnectionString, 
            blobDirName);
        var blobs = await azureStorageHelper.GetBlobsAsync();
        return blobs.ToArray();
    }
    
    [FunctionName("CalculateSize")]
    public async Task<int> CalculateSize([ActivityTrigger]string blobName, ILogger log)
    {
        log.LogInformation($"Getting blob {blobName} and reading lines of files");
        var azureStorageHelper = new AzureStorageHelper(storageConnectionString, 
            currentBlobDirectory);
        
        return await azureStorageHelper.NumberOfLinesInBlobAsync(blobName);
    }
}