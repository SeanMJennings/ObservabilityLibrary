using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using SerilogILogger = Serilog.ILogger;

namespace Logging.Serilog.OpenTelemetry;

public static class SerilogOpenTelemetryLogger
{
    // Must be kept alive for the lifetime of the application
    private static ILoggerFactory TheLoggerFactory = null!;
    
    public static void ConfigureSerilogAndOpenTelemetry(string applicationName, string instrumentationKey)
    {
        TheLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = instrumentationKey);
            });
        });

        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithExceptionDetails()
            .WriteTo.OpenTelemetry()
            .WriteTo.Conditional(
                _ => System.Diagnostics.Debugger.IsAttached,
                wt => wt.Console()
            ).CreateLogger();
        
        TheLoggerFactory.AddSerilog(logger);
        var theLogger = TheLoggerFactory.CreateLogger<SerilogILogger>();
        Logger.Configure(theLogger);
    }    
} 