using BDD;
using Logging.Serilog.OpenTelemetry.AspNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Shouldly;

namespace Testing;

public partial class SerilogSetupShould : Specification
{
    private WebApplicationBuilder webApplicationBuilder = null!;
    private WebApplication webApplication = null!;
    private Serilog.Core.Logger logger = null!;
    
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
        webApplicationBuilder.ConfigureSerilogAndOpenTelemetry("wibble", "wobble");
        webApplication = webApplicationBuilder.Build();
        webApplication.UseSerilogForHttpRequestLogging();
        webApplication.ConfigureSerilogGlobally();
    }

    private void serilog_is_configured_for_aspnet()
    {
        webApplication.Services.ShouldNotBeNull();
        webApplication.Services.GetService<ILogger<Serilog.ILogger>>().ShouldBeOfType<Logger<Serilog.ILogger>>();
    }
}