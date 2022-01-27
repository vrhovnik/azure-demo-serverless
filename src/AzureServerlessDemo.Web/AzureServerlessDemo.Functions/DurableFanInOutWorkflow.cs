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

        if (string.IsNullOrEmpty(currentBlobDirectory)) currentBlobDirectory = "durableblobs";
        
        var result = await starter.StartNewAsync("DoCalculationsFun", blobDirectory);
             
        var status = await starter.GetStatusAsync(result);
        do
        {
            //do long pooling, but we have other means as well
            log.LogInformation($"Checking status in 3 seconds, current status {status.RuntimeStatus}");
            await Task.Delay(TimeSpan.FromSeconds(3));
            status = await starter.GetStatusAsync(result);
        }
        while (status.RuntimeStatus != OrchestrationRuntimeStatus.Completed);

        log.LogInformation("Result is finished");

        var output = status.Output;              
        return new OkObjectResult(output);
    }

    [FunctionName("DoCalculationsFun")]
    public async Task<int> DoCalculationsFun([OrchestrationTrigger] IDurableOrchestrationContext activityContext,
        CancellationToken ctk, ILogger log)
    {
        var directoryName = activityContext.GetInput<string>();

        if (string.IsNullOrEmpty(directoryName)) directoryName = "durableblobs";

        log.LogInformation("Current directory name is: " + directoryName);

        var parralelTasks = new List<Task<int>>();

        var blobNames = await activityContext.CallActivityAsync<string[]>("GetBlobNames", directoryName);

        foreach (var currentBlobName in blobNames)
        {
            log.LogInformation($"Adding calculation for {currentBlobName}");
            var task = activityContext.CallActivityAsync<int>("CalculateSize", currentBlobName);
            parralelTasks.Add(task);
        }

        await Task.WhenAll(parralelTasks);

        log.LogInformation($"Task were finished, results get calculated for {parralelTasks.Count} blobs");

        int size = parralelTasks.Sum(currentBlobSize => currentBlobSize.GetAwaiter().GetResult());

        log.LogInformation($"Line size is {size}");

        return size;
    }

    [FunctionName("GetBlobNames")]
    public async Task<string[]> GetBlobNames([ActivityTrigger] string blobDirName, ILogger log)
    {
        if (string.IsNullOrEmpty(blobDirName)) blobDirName = "durableblobs";

        log.LogInformation($"Getting blob names from container {blobDirName}");

        var azureStorageHelper = new AzureStorageHelper(storageConnectionString,
            blobDirName);
        var blobs = await azureStorageHelper.GetBlobsAsync();
        log.LogInformation($"Received {blobs.Count} blobs for calculation.");
        return blobs.ToArray();
    }

    [FunctionName("CalculateSize")]
    public async Task<int> CalculateSize([ActivityTrigger] string blobName, ILogger log)
    {
        log.LogInformation($"Getting blob {blobName} and reading lines of files");
        
        if (string.IsNullOrEmpty(currentBlobDirectory)) currentBlobDirectory = "durableblobs";
        
        var azureStorageHelper = new AzureStorageHelper(storageConnectionString,
            currentBlobDirectory);

        log.LogInformation($"Gettig info about number of lines for {blobName}");
        
        return await azureStorageHelper.NumberOfLinesInBlobAsync(blobName);
    }
}