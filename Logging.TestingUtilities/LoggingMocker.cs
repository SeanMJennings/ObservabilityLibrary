using Alerting;

namespace Logging.TestingUtilities
{
    public static class LoggingMocker
    {
        public static void SetupLoggingInfoMock(Action<string, object?> logInformationAction)
        {
            Logger.LogInformationAction = logInformationAction;
        }         
        
        public static void SetupLoggingWarningMock(Action<string, object?> logWarningAction)
        {
            Logger.LogWarningAction = logWarningAction;
        } 
        
        public static void SetupLoggingErrorMock(Action<string, Exception, AlertCode, object?> logErrorAction)
        {
            Logger.LogErrorAction = logErrorAction;
        }
    }
}