using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using SerilogILogger = Serilog.ILogger;

namespace Logging.Serilog.OpenTelemetry;

public static class SerilogOpenTelemetryLogger
{
    // Must be kept alive for the lifetime of the application
    private static ILoggerFactory? TheLoggerFactory;
    
    public static void ConfigureOpenTelemetry(string connectionString)
    {
        TheLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = connectionString);
            });
        });
    }      
    
    public static void ConfigureSerilog(string applicationName)
    {
        TheLoggerFactory ??= new LoggerFactory();

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