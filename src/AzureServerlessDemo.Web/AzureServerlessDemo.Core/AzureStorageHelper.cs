using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureServerlessDemo.Core;

public class AzureStorageHelper
{
    private readonly string containerName;
    private readonly BlobServiceClient blobServiceClient;

    public AzureStorageHelper(string connectionString, string containerName)
    {
        this.containerName = containerName;
        blobServiceClient = new BlobServiceClient(connectionString);
    }

    public async Task<List<string>> GetBlobsAsync()
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        await blobContainerClient.CreateIfNotExistsAsync();

        var list = new List<string>(); 
        await foreach (var blobItem in blobContainerClient.GetBlobsAsync())
        {
            list.Add(blobItem.Name);
        }

        return list;
    }

    public async Task<int> NumberOfLinesInBlobAsync(string blobName)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = blobContainerClient.GetBlobClient(blobName);
        var downloadedContent = await blobClient.DownloadAsync();
        var lineCount = 0;
        using var streamReader= new StreamReader(downloadedContent.Value.Content);
        while (!streamReader.EndOfStream)
        {
            await streamReader.ReadLineAsync();
            lineCount++;
        }

        return lineCount;
    }
}