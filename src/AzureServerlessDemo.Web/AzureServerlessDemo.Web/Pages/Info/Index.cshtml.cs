using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AzureServerlessDemo.Web.Pages.Info
{
    public class IndexPageModel : PageModel
    {
        private readonly ILogger<IndexPageModel> logger;

        public IndexPageModel(ILogger<IndexPageModel> logger)
        {
            this.logger = logger;
        }

        public void OnGet()
        {
            logger.LogInformation("Info page loaded");
        }
    }
}