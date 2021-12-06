using AzureServerlessDemo.Web.Hubs;
using AzureServerlessDemo.Web.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
        options.Conventions.AddPageRoute("/Info/Index", ""));

builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.AddHealthChecks();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health").AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapHub<AlertHub>("/alerts");
});

app.Run();