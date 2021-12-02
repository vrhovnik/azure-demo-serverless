using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AzureServerlessDemo.Functions;

public static class SimpleWorkflow
{
    [FunctionName("SimpleWorkflow")]
    public static async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{message}")]
        HttpRequest req, string message, ILogger log)
    {
        log.LogInformation("Simple workflow to prepare message and then send an email to all of our subscribers");

        try
        {
            var recepeintsEmails = Environment.GetEnvironmentVariable("default-emails");

            var emails = recepeintsEmails?.Split(";");

            //get message based on query string
            var messageBody = "<h1>Please check your profile data and let us know, if any questions</h1>";
            if (!string.IsNullOrEmpty(message))
            {
                switch (messageBody)
                {
                    case "data":
                        messageBody = "<h1>Please check your portal as we brought to you new features</h1>";
                        break;
                    case "info":
                        messageBody = "<h1>You have used your quota to the fullest. Increase to max to get service working.</h1>";
                        break;
                    default:
                        messageBody = "<p>Please call us to explain the products</p>";
                        break;
                }
            }
            
            var stopwatch = Stopwatch.StartNew();

            //send message
            var sendGridClient = new SendGridClient(Environment.GetEnvironmentVariable("SendGridApiKey"));

            foreach (var currentEmail in emails)
            {
                var sendGridEmailMessage = new SendGridMessage();
                sendGridEmailMessage.AddTo(currentEmail);
                sendGridEmailMessage.AddContent("text/html", messageBody);
                sendGridEmailMessage.SetFrom(new EmailAddress("info@azuredemos.net"));
                sendGridEmailMessage.SetSubject("Data about usage");
                await sendGridClient.SendEmailAsync(sendGridEmailMessage);
            }

            stopwatch.Stop();

            var logMessage = $"Sending email to {emails.Length} clients and it took {stopwatch.ElapsedMilliseconds}";
            log.LogInformation(logMessage);

            return new OkObjectResult(logMessage);
        }
        catch (Exception e)
        {
            log.LogError(e.Message);
            return new BadRequestResult();
        }
    }
}