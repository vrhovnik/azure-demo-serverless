using Azure.Data.Tables;
using AzureServerlessDemo.Core;
using AzureServerlessDemo.Web.Options;
using Htmx;
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

    public async Task<IActionResult> OnGetAsync(string query)
    {
        logger.LogInformation("Loaded page LogsPageModel with storage options");
        
        FunctionURL = Environment.GetEnvironmentVariable("FunctionURL");
        logger.LogInformation("Function URL is " + FunctionURL);
        
        logger.LogInformation($"Conn string for Azure Table {storageOptions.ConnectionString} on table {storageOptions.TableName}");

        var client = new TableClient(storageOptions.ConnectionString, storageOptions.TableName);
        await client.CreateIfNotExistsAsync();
        
        var queryTableResults = client.QueryAsync<LogModel>($"PartitionKey eq 'serverlesslogs'");

        await foreach (var currentResult in queryTableResults)
        {
            Logs.Add(currentResult);
        }

        if (!string.IsNullOrEmpty(query))
            Logs = Logs.Where(currentLog =>
                currentLog.Text.Contains(query) || currentLog.CalledFromMethod.Contains(query))
                .ToList();
        
        if (Request.IsHtmx())
            return Partial("_LogsList", Logs);
        
        return Page();
    }

    [BindProperty] public List<LogModel> Logs { get; set; } = new();
    [BindProperty] public string FunctionURL { get; set; } = string.Empty;
}