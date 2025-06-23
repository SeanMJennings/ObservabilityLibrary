using BDD;
using NUnit.Framework;

namespace Testing;

[TestFixture]
public partial class LoggerShould : Specification
{
    [Test]
    public void log_information()
    {
        Given(the_logger_is_configured);
        When(logging_information);
        Then(the_info_is_logged);
    }    
    
    [Test]
    public void log_warning()
    {
        Given(the_logger_is_configured);
        When(logging_warning);
        Then(the_warning_is_logged);
    }

    [Test]
    public void log_error_that_will_not_trigger_an_alert()
    {
        Given(the_logger_is_configured);
        When(logging_error_that_will_not_trigger_an_alert);
        Then(the_error_is_logged);
        Then(the_error_is_marked_as_ignored_for_alerting);
    }    
    
    [Test]
    public void log_error_that_will_trigger_an_alert()
    {
        Given(the_logger_is_configured);
        When(logging_error_that_will_generate_an_alert);
        Then(the_error_is_logged);
        Then(the_error_is_marked_as_an_alert);
    }
}