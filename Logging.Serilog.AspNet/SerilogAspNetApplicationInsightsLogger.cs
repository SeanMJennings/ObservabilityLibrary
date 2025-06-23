using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;

namespace Logging.Serilog.Aspnet;

public static class SerilogAspNetApplicationInsightsLogger
{
    public static void ConfigureSerilogLoggingApplicationInsightsForAspNet(this WebApplicationBuilder builder, string instrumentationKey, string applicationName)
    {
        builder.Services.AddApplicationInsightsTelemetryWorkerService(options =>
        {
            options.ConnectionString = instrumentationKey;
        });
        builder.Host.UseSerilog((_, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .MinimumLevel.Information()
                .Enrich.WithProperty("Application", applicationName)
                .Enrich.WithExceptionDetails()
                .WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(),
                    TelemetryConverter.Traces);
        });
    }

    public static void ConfigureSerilogGlobally(this IServiceProvider serviceProvider)
    {
        var serilogLogger = serviceProvider.GetService<ILogger>();
        if (serilogLogger is null) throw new ApplicationException("Serilog logger has not been registered");
        Logger.Configure(serilogLogger);
    }

    public static void UseSerilogForHttpRequestLogging(this WebApplication webApplication) => webApplication.UseSerilogRequestLogging();
}