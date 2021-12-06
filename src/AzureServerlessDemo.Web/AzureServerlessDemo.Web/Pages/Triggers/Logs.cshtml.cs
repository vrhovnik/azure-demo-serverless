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

    public void OnGet()
    {
        logger.LogInformation("Loaded page LogsPageModel with storage options");
        logger.LogInformation($"Conn string: {storageOptions.ConnectionString} on table {storageOptions.TableName}");
    }

    [BindProperty]
    public List<LogModel> Logs { get; set; } = new();
}