using Azure.Data.Tables;
using AzureServerlessDemo.Core;
using AzureServerlessDemo.Web.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AzureServerlessDemo.Web.Pages.Triggers;

public class LogsPageModel : PageModel
{
    private readonly ILogger<LogsPageModel> logger;
    private readonly StorageOptions? storageOptions;

    public LogsPageModel(IOptions<StorageOptions> storageOptionsValue, ILogger<LogsPageModel> logger)
    {
        storageOptions = storageOptionsValue.Value;
        this.logger = logger;
    }

    public async Task OnGetAsync()
    {
        logger.LogInformation("Loaded page LogsPageModel with storage options");
        logger.LogInformation($"Conn string: {storageOptions.ConnectionString} on table {storageOptions.TableName}");
        
        var client = new TableClient(storageOptions.ConnectionString, storageOptions.TableName);
        await client.CreateIfNotExistsAsync();
        
        var queryTableResults = client.QueryAsync<LogModel>($"TableName eq '{storageOptions.TableName}'");
        
        Console.WriteLine("The following are the names of the tables in the query result:");
        await foreach (var currentResult in queryTableResults)
        {
            Logs.Add(currentResult);
        }
    }

    [BindProperty] public List<LogModel> Logs { get; } = new();
}