using ServiceContracts;
using Microsoft.EntityFrameworkCore;
using Entities;

using Services;
using OfficeOpenXml;

using RepositoryContracts;
using Repositories;
using Serilog;
using CRUDExample.Filters.ActionFilters;
using CRUDExample.Filters.PersonsListResultFilter;
using CRUDExample;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
    loggerConfiguration
    .ReadFrom.Configuration(context.Configuration) // read configuration settings from built-in IConfiguration
    .ReadFrom.Services(services); // read current app's services and make them available to serilog
});

builder.Services.ConfigureServices(builder.Configuration, builder.Environment);

ExcelPackage.License.SetNonCommercialPersonal("Scott Miner");
var app = builder.Build();

app.UseSerilogRequestLogging();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpLogging();

if (builder.Environment.IsEnvironment("Test") == false)
{
    Rotativa.AspNetCore.RotativaConfiguration.Setup("wwwroot", "Rotativa");
}

//app.Logger.LogDebug("debug-message");
//app.Logger.LogInformation("information-message");
//app.Logger.LogWarning("warning-message");
//app.Logger.LogError("error-message");
//app.Logger.LogCritical("critical-message");

app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();

public partial class Program


{ } // make the auto-generated Program accessible programmatically