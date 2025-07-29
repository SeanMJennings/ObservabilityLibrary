using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;

namespace Observability.OpenTelemetry.AspNet;

public static class OpenTelemetrySetup
{
    public static OpenTelemetryBuilder ConfigureOpenTelemetry(this WebApplicationBuilder builder, string applicationName)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddOpenTelemetry(opt =>
        {
            opt.IncludeFormattedMessage = true;
            opt.IncludeScopes = true;
            opt.ParseStateValues = true;
            opt.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName)); // Match the trace service name
        });
        
        return builder.Services.AddOpenTelemetry();
    }

    public static OpenTelemetryBuilder UseAzureMonitor(this OpenTelemetryBuilder builder, string connectionString)
    {
        return builder.UseAzureMonitor(m => m.ConnectionString = connectionString);
    }

    public static void ConfigureGlobalLogger(this WebApplication webApplication)
    {
        var logger = webApplication.Logger;
        if (logger is null) throw new ApplicationException("Logger has not been registered");
        Logger.Configure(logger);
    }
}