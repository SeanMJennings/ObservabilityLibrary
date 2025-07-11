using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using SerilogCore = Serilog;
using Serilog.Exceptions;

namespace Logging.Serilog.OpenTelemetry.AspNet;

public static class SerilogOpenTelemetryLogger
{
    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder, string instrumentationKey)
    {
        builder.Services.AddOpenTelemetry().UseAzureMonitor(m => m.ConnectionString = instrumentationKey);
    }    
    
    public static void ConfigureSerilog(this WebApplicationBuilder builder, string applicationName)
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithExceptionDetails()
            .WriteTo.OpenTelemetry()
            .WriteTo.Conditional(
                _ => System.Diagnostics.Debugger.IsAttached,
                wt => wt.Console()
            ).CreateLogger();
        
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger, dispose: true);
        builder.Host.UseSerilog(logger);
    }

    public static void ConfigureSerilogGlobally(this WebApplication webApplication)
    {
        var serilogLogger = webApplication.Services.GetService<ILogger<SerilogCore.ILogger>>();
        if (serilogLogger is null) throw new ApplicationException("Serilog logger has not been registered");
        Logger.Configure(serilogLogger);
    }

    public static void UseSerilogForHttpRequestLogging(this WebApplication webApplication) => webApplication.UseSerilogRequestLogging();
}