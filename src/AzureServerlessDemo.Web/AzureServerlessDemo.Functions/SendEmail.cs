using System.Threading.Tasks;
using System;
using AzureServerlessDemo.Core;
using AzureServerlessDemo.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;

namespace AzureServerlessDemo.Functions;

public static class SendEmail
{
    [FunctionName("SendEmail")]
    public static async Task RunAsync([QueueTrigger("azure-serverless-emails", Connection = "")] string emailObject, 
        [SendGrid(ApiKey = "SendGridApiKey")]IAsyncCollector<SendGridMessage> messageCollector,
        [Table("serverlesslogs")]IAsyncCollector<LogModel> tableCollector,
        ILogger log)
    {
        log.LogInformation($"Sending email to object {emailObject}");
        var recepeintsEmails = Environment.GetEnvironmentVariable("default-emails");
        var emails = recepeintsEmails?.Split(";");
        var emailModel = JsonConvert.DeserializeObject<EmailModel>(emailObject); 
        
        //get message based on query string
        var messageBody = "<h1>Please check your profile data and let us know, if any questions</h1>";
        if (!string.IsNullOrEmpty(emailModel.Message))
        {
            messageBody = emailModel.Message switch
            {
                "data" => "<h1>Please check your portal as we brought to you new features</h1>",
                "info" =>
                    "<h1>You have used your quota to the fullest. Increase to max to get service working.</h1>",
                _ => "<p>Please call us to explain the products</p>"
            };
        }
        foreach (var email in emails)
        {
            var message = new SendGridMessage();
            message.AddTo(email);
            message.AddContent("text/html", messageBody);
            message.SetFrom(new EmailAddress("info@azuredemos.net"));
            message.SetSubject(emailModel.Subject);
            log.LogInformation("Sending email to client...");
            
            await messageCollector.AddAsync(message);
            
            log.LogInformation("Addding data to Azure Tables for log purposes");
            
            var logItem = new LogModel
            {
                PartitionKey = "logs",
                RowKey = Guid.NewGuid().ToString(),
                Text = $"Sending email from queue to {email} with message {messageBody}",
                CalledFromMethod = "SendEmail",
                LoggedDate = DateTime.Now
            };
            
            await tableCollector.AddAsync(logItem);
        }
    }
}