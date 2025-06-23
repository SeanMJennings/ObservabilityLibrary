using NUnit.Framework;

namespace Testing;

[TestFixture]
public partial class SerilogSetupShould
{
    [Test]
    public void configure_serilog_for_aspnet_and_app_insights()
    {
        Given(web_app_builder);
        When(configuring);
        Then(serilog_is_configured_for_aspnet);
    }
}