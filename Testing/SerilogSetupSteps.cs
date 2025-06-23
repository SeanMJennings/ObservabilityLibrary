using BDD;
using Logging.Serilog.Aspnet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shouldly;

namespace Testing;

public partial class SerilogSetupShould : Specification
{
    private WebApplicationBuilder webApplicationBuilder;
    private WebApplication webApplication;
    private Serilog.Core.Logger logger;
    
    protected override void before_each()
    {
        webApplicationBuilder = null!;
        webApplication = null!;
        logger = null!;
    }
    private void web_app_builder()
    {
        webApplicationBuilder = WebApplication.CreateBuilder();
    }

    private void configuring()
    {
        logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .CreateLogger();
        webApplicationBuilder.ConfigureSerilogLoggingApplicationInsightsForAspNet("wibble", "wobble");
        webApplication = webApplicationBuilder.Build();
        webApplication.Services.ConfigureSerilogGlobally();
    }

    private void serilog_is_configured_for_aspnet()
    {
        webApplication.Services.ShouldNotBeNull();
        webApplication.Services.GetService<ILogger>().ShouldBeOfType<Serilog.Core.Logger>();
    }
}