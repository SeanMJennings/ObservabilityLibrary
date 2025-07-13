using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;

namespace Observability.OpenTelemetry.AspNet;

public static class OpenTelemetrySetup
{
    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder, string connectionString, string applicationName)
    {
        builder.Services.AddOpenTelemetry().UseAzureMonitor(m => m.ConnectionString = connectionString);
        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(opt =>
        {
            opt.IncludeFormattedMessage = true;
            opt.IncludeScopes = true;
            opt.ParseStateValues = true;
            opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName)); // Match the trace service name
        });
    }    

    public static void ConfigureGlobalLogger(this WebApplication webApplication)
    {
        var logger = webApplication.Logger;
        if (logger is null) throw new ApplicationException("Logger has not been registered");
        Logger.Configure(logger);
    }
}