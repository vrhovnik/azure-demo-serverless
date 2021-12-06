using Microsoft.AspNetCore.SignalR;

namespace AzureServerlessDemo.Web.Hubs;

public class AlertHub : Hub
{
    public Task AlertMessage(string message) =>
        Clients.All.SendAsync("alertMessage", message);
}