using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.SystemEvents;
using AzureServerlessDemo.Web.Hubs;
using AzureServerlessDemo.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace AzureServerlessDemo.Web.Controllers;

[ApiController]
[Route("notification")]
public class AlertController : ControllerBase
{
    private readonly IHubContext<AlertHub> hubContext;
    private readonly ILogger<AlertController> logger;

    public AlertController(IHubContext<AlertHub> hubContext, ILogger<AlertController> logger)
    {
        this.hubContext = hubContext;
        this.logger = logger;
    }

    [Route("check")]
    public IActionResult Health() => Ok($"I am alive {DateTime.Now}");

    [HttpPost]
    [Route("alert")]
    public async Task<IActionResult> NotifyWebPageWithSignalR()
    {
        logger.LogInformation("Reading from body");
        try
        {
            EventGridEvent[] egEvents = EventGridEvent.ParseMany(BinaryData.FromStream(HttpContext.Request.Body));
            logger.LogInformation("Got events - check if validation code is present");
            foreach (var eventGridEvent in egEvents)
            {
                if (eventGridEvent.TryGetSystemEventData(out object systemEvent) &&
                    systemEvent is SubscriptionValidationEventData subscriptionValidated)
                {
                    logger.LogInformation("Validation code: " + subscriptionValidated.ValidationCode);
                    var responseData =
                        new SubscriptionValidationResponseData
                            { ValidationResponse = subscriptionValidated.ValidationCode };
                    return Ok(JsonConvert.SerializeObject(responseData));
                }
            
                logger.LogInformation("Reading data property " + eventGridEvent.Data);
                await hubContext.Clients.All.SendAsync("alertMessage", eventGridEvent.Data.ToString());    
            }

            logger.LogInformation("Done calling events from NotifyWebPageWithSignalR");
            return Ok($"Data was received at {DateTime.Now} and all clients has been notified.");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return BadRequest(e.Message);
        }
    }
}