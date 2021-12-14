# Demo repository to demonstrate Azure Serverless solutions

Demo repository to demonstrate Azure Serverless solutions, which demonstrate how to leverage simple application and then extend it with the use of serverless.

![Azure Serverless in a Nutshell](https://webeudatastorage.blob.core.windows.net/web/azure-serverless.png)

## STRUCTURE

The demo solution is built with 3 parts in mind:

![Demo solution structure](https://webeudatastorage.blob.core.windows.net/web/azure-serverless-structure.png)

Each of them represents functionalities:
1. **Core** - for common libraries and models to be used by functions and GUI
2. **Func** - Azure Functions code for demonstrating different functionalities.
3. **Web** - code for web user interface to be able to execute demo scenarios 

## INSTRUCTIONS

In order to run the project locally you will need to have [.NET 6](https://dot.net) installed and configured [runtime for running Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local). Follow the instructions [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-local) how to set it up.

You can pick whatever tool you fit appropriately. We do recommend [Visual Studio](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs) or [Visual Studio Code](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code).

Open solution [AzureServelessDemo.Web.sln](https://github.com/vrhovnik/azure-demo-serverless/blob/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Web.sln) in tool of your choice to see the code. In order to run it locally, you will need to setup environment variables in:
1. [AzureServerlessDemo.Functions](https://github.com/vrhovnik/azure-demo-serverless/tree/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Functions):
   * [SendEmail](https://github.com/vrhovnik/azure-demo-serverless/blob/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Functions/SendEmail.cs)
      * **SendGridApiKey** - key for SendGrid service, which will be used for sending emails
      * **default-emails** - emails separated with ;
   * [BetterWorkflow](https://github.com/vrhovnik/azure-demo-serverless/blob/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Functions/BetterWorkflow.cs)
      * **SendGridApiKey** - key for SendGrid service, which will be used for sending emails
      * **default-emails** - emails separated with ;
      * **AzureWebJobsStorage** - connection string for Azure Storage Account (or leave UseDevelopmentStorage=true for local development)
   * [TighterIntegrationWorkflow](https://github.com/vrhovnik/azure-demo-serverless/blob/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Functions/TighterIntegrationWorkflow.cs):
      * **EventGridNotificationEndpoint** - endpoint url to handle notification for this scenario
      * **EventGridKey** - key to authenticate with Azure Event Grid
2. [AzureServerlessDemo.Web](https://github.com/vrhovnik/azure-demo-serverless/tree/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Web):
   * **StorageOptions__ConnectionString** - connection string to Azure Storage or local development storage UseDevelopmentStorage=true - check [appsettings.json](https://github.com/vrhovnik/azure-demo-serverless/blob/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Web/appsettings.json) as well for additional requirements:
   * **FunctionURL** - url pointing to the base URL for Azure Function
   * **SimpleWorkflowUrl** - url pointing to workflow to execute simple scenarios
   * **BetterWorkflowUrl** - url pointing to workflow to execute showcase, how to do better scenario based on simple scenario
   * **TighterWorkflowUrl** - url pointing to workflow to execute scenario with tighter integration with Azure services
   * **Azure__SignalR__ConnectionString** - Azure SignalR connection string for realtime updates

When you have that configured, you can run [web project](https://github.com/vrhovnik/azure-demo-serverless/tree/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Web) and [functions project](https://github.com/vrhovnik/azure-demo-serverless/tree/main/src/AzureServerlessDemo.Web/AzureServerlessDemo.Functions) separately to see the execution and GUI respectively.

How to run web project --> described [here](https://dotnet.microsoft.com/en-us/learn/aspnet/hello-world-tutorial/run). 
How to run functions project --> described [here](https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp#run-functions).

## CREDIT

In the application itself (for the presentation and for management) I used:

1. [HTMX](https://htmx.org/docs/) for accessing modern browser features and [helpers from Khalid Abuhmed](https://khalidabuhakmeh.com/htmx-with-aspnet-core)
2. [Bootstrap 5](https://getboostrap.com) for CSS framework for GUI

# Additional Information

1. [Azure Serverless](https://azure.microsoft.com/en-us/solutions/serverless/)
2. [Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-overview)
3. [Azure Samples](https://github.com/Azure-Samples)
