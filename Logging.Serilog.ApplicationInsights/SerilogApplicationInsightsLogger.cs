using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Logging;
using Serilog.Exceptions;

namespace Logging.Serilog.ApplicationInsights;

public static class SerilogApplicationInsightsLogger
{
    public static void ConfigureGlobalLoggingWithBuiltServices(string applicationName, string instrumentationKey, IServiceCollection serviceCollection)
    {
        serviceCollection.AddLogging(a => a.SetMinimumLevel(LogLevel.None));
        serviceCollection.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.ConnectionString = instrumentationKey;
        });
        
        var serviceProvider = serviceCollection.BuildServiceProvider();
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("Application", applicationName)
            .Enrich.WithExceptionDetails()
            .WriteTo.ApplicationInsights(serviceProvider.GetRequiredService<TelemetryConfiguration>(), TelemetryConverter.Traces)
            .CreateLogger();
            
        Logger.Configure(logger);
    }    
    
    public static void ConfigureGlobalLoggingWithBuiltServices(string applicationName, string instrumentationKey)
    {
        var services = new ServiceCollection();
        ConfigureGlobalLoggingWithBuiltServices(applicationName, instrumentationKey, services);
    }
} 