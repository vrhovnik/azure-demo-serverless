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
        logger.LogInformation("Page WorkflowPageModel loaded, loading environment variables");
        
        SimpleWorkflowUrl = Environment.GetEnvironmentVariable("SimpleWorkflowUrl");
        logger.LogInformation("Simple: " + SimpleWorkflowUrl);
        EnhancedWorkflowUrl = Environment.GetEnvironmentVariable("EnhancedWorkflowUrl");
        logger.LogInformation("Enhanced: " + EnhancedWorkflowUrl);
        TighterIntegration = Environment.GetEnvironmentVariable("TighterIntegration");
        logger.LogInformation("Tighter: " + TighterIntegration);
    }
    
    [BindProperty]
    public string SimpleWorkflowUrl { get; set; } 
    
    [BindProperty]
    public string EnhancedWorkflowUrl { get; set; }
    
    [BindProperty]
    public string TighterIntegration { get; set; }
}