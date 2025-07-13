using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Resources;

namespace OpenTelemetry;

public static class OpenTelemetry
{
    // Must be kept alive for the lifetime of the application
    private static ILoggerFactory? TheLoggerFactory;
    
    public static void ConfigureOpenTelemetry(string connectionString, string applicationName)
    {
        TheLoggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddOpenTelemetry(logging =>
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = connectionString);
                logging.IncludeFormattedMessage = true;
                logging.IncludeScopes = true;
                logging.ParseStateValues = true;
                logging.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(applicationName)); // Match the trace service name
            });
        });
        var theLogger = TheLoggerFactory.CreateLogger("Default");
        Logger.Configure(theLogger);
    }
} 