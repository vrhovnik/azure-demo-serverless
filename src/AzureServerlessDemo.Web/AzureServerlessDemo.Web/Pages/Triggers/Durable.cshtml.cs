using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServerlessDemo.Web.Pages.Triggers;

public class DurablePageModel : PageModel
{
    private readonly ILogger<DurablePageModel> logger;

    public DurablePageModel(ILogger<DurablePageModel> logger)
    {
        this.logger = logger;
    }

    [BindProperty]
    public string SimpleDurableUrl { get; set; }

    [BindProperty]
    public string FanInOutDurableUrl { get; set; }

    public void OnGet()
    {
        logger.LogInformation("Page DurablePageModel loaded");
        SimpleDurableUrl = Environment.GetEnvironmentVariable("SimpleDurableUrl");
        logger.LogInformation("Simple durable url: " + SimpleDurableUrl);
        FanInOutDurableUrl = Environment.GetEnvironmentVariable("FanInOutDurableUrl");
        logger.LogInformation("Fan in/out durable url: " + FanInOutDurableUrl);
    }
}