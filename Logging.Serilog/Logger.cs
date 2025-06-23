using Alerting;
using Serilog.Events;
using SerilogILogger = Serilog.ILogger;

public static class Logger
{
    private static SerilogILogger TheLogger = null!;
        
    public static void Configure(SerilogILogger logger)
    {
        TheLogger = logger;
    }
        
    public static void LogInformation(string message, object? additionalInformation = null) => LogInformationAction(message, additionalInformation);
    public static void LogWarning(string message, object? additionalInformation = null) => LogWarningAction(message, additionalInformation);
    public static void LogErrorThatWillNotTriggerAnAlert(Exception exception, object? additionalInformation = null) => LogErrorAction(exception.Message, exception, AlertCode.Ignore, additionalInformation);
    public static void LogErrorThatWillTriggerAnAlert(Exception exception, object? additionalInformation = null) => LogErrorAction(exception.Message, exception, AlertCode.Alert, additionalInformation);
    public static void LogErrorThatWillNotTriggerAnAlert(string message, Exception exception, object? additionalInformation = null) => LogErrorAction(message, exception, AlertCode.Ignore, additionalInformation);
    public static void LogErrorThatWillTriggerAnAlert(string message, Exception exception, object? additionalInformation = null) => LogErrorAction(message, exception, AlertCode.Alert, additionalInformation);

    public static async Task CloseAndFlush()
    {
        ((IDisposable)TheLogger).Dispose();
        await Task.Delay(5000); //https://learn.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#flushing-data
    } 
        
    internal static Action<string, object?> LogInformationAction = (message, additionalInformation) =>
    {
        TheLogger.ForContext("AdditionalInformation", additionalInformation)
            .Write(LogEventLevel.Information, message);
    };
        
    internal static Action<string, object?> LogWarningAction = (message, additionalInformation) =>
    {
        TheLogger.ForContext("AdditionalInformation", additionalInformation)
            .Write(LogEventLevel.Warning, message);
    };
        
    internal static Action<string, Exception, AlertCode,  object?> LogErrorAction = (message, exception, alertCode, additionalInformation) =>
    {
        TheLogger.ForContext("AdditionalInformation", additionalInformation)
            .ForContext("AlertCode", alertCode)
            .Write(LogEventLevel.Error, exception, message);
    };
}