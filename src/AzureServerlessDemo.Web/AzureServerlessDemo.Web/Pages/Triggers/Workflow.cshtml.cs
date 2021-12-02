using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServerlessDemo.Web.Pages.Triggers;

public class WorkflowPageModel : PageModel
{
    private readonly ILogger<WorkflowPageModel> logger;

    public WorkflowPageModel(ILogger<WorkflowPageModel> logger)
    {
        this.logger = logger;
    }

    public void OnGet()
    {
        logger.LogInformation("Page WorkflowPageModel loaded");
        SimpleWorkflowUrl = Environment.GetEnvironmentVariable("SimpleWorkflowUrl");
    }
    
    [BindProperty]
    public string SimpleWorkflowUrl { get; set; }
}