using Alerting;
using Moq;
using Shouldly;
using Logging.TestingUtilities;
using Microsoft.Extensions.Logging;
using SerilogILogger = Serilog.ILogger;

namespace Testing;

public partial class LoggerShould
{
    private Mock<ILogger<SerilogILogger>> MockLogger = null!;
    private const string Message = "message";
    private const string WarningMessage = "message-warning";
    private MockException Exception = null!;
    private object AdditionalInformation = null!;
    private string capturedMessage = null!;
    private object capturedAdditionalInformation = null!;
    private Exception capturedException = null!;
    private AlertCode capturedAlertCode;
    
    private class MockException(string message) : Exception(message);

    protected override void before_each()
    {
        base.before_each();
        capturedMessage = null!;
        capturedAdditionalInformation = null!;
        capturedException = null!;
        MockLogger = new Mock<ILogger<SerilogILogger>>();
        AdditionalInformation = new { PropertyWeCareAbout = "wobble" };
        Exception = new MockException("exception");
        var logInformationAction = new Action<string, object?>((theMessage, theAdditionalInformation) =>
        {
            capturedMessage = theMessage;
            capturedAdditionalInformation = theAdditionalInformation!;
        });
        var logWarningAction = new Action<string, object?>((theMessage, theAdditionalInformation) =>
        {
            capturedMessage = theMessage;
            capturedAdditionalInformation = theAdditionalInformation!;
        });
        var logErrorAction = new Action<string, Exception, AlertCode, object?>((message, theException, alertCode, theAdditionalInformation) =>
        {
            capturedMessage = message;
            capturedException = theException;
            capturedAdditionalInformation = theAdditionalInformation!;
            capturedAlertCode = alertCode;
        });
        LoggingMocker.SetupLoggingInfoMock(logInformationAction);
        LoggingMocker.SetupLoggingWarningMock(logWarningAction);
        LoggingMocker.SetupLoggingErrorMock(logErrorAction);
    }

    private void the_logger_is_configured()
    {
        Logger.Configure(MockLogger.Object);
    }

    private void logging_information()
    {
        Logger.LogInformation(Message, AdditionalInformation);
    }    
    
    private void logging_warning()
    {
        Logger.LogWarning(WarningMessage, AdditionalInformation);
    }

    private void the_info_is_logged()
    {
        capturedMessage.ShouldBe(Message);
        capturedAdditionalInformation.ShouldBe(AdditionalInformation);
    }    
    
    private void the_warning_is_logged()
    {
        capturedMessage.ShouldBe(WarningMessage);
        capturedAdditionalInformation.ShouldBe(AdditionalInformation);
    }

    private void logging_error_that_will_not_trigger_an_alert()
    {
        Logger.LogErrorThatWillNotTriggerAnAlert(Exception, AdditionalInformation);
    }
    
    private void logging_error_that_will_generate_an_alert()
    {
        Logger.LogErrorThatWillTriggerAnAlert(Exception, AdditionalInformation);
    }
    
    private void the_error_is_logged()
    {
        capturedMessage.ShouldBe(Exception.Message);
        capturedException.ShouldBe(Exception);
        capturedAdditionalInformation.ShouldBe(AdditionalInformation);
    }
    
    private void the_error_is_marked_as_an_alert()
    {
        capturedAlertCode.ShouldBe(AlertCode.Alert);
    }
    
    private void the_error_is_marked_as_ignored_for_alerting()
    {
        capturedAlertCode.ShouldBe(AlertCode.Ignore);
    }
}