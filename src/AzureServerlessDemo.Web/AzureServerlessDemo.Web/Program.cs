using AzureServerlessDemo.Web.Hubs;
using AzureServerlessDemo.Web.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
        options.Conventions.AddPageRoute("/Info/Index", ""));

builder.Services.AddSignalR().AddAzureSignalR();
builder.Services.AddHealthChecks();
builder.Services.AddControllers();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "Azure Allowed",
        builder =>
        {
            builder.WithOrigins("https://*.azurewebsites.net") 
                .SetIsOriginAllowedToAllowWildcardSubdomains();
            //builder.WithOrigins("https://locahost").AllowAnyMethod();
        });
});


var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseCors();

app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/health")
        .AllowAnonymous();
    endpoints.MapRazorPages();
    endpoints.MapControllers();
    endpoints.MapHub<AlertHub>("/alerts");
});

app.Run();