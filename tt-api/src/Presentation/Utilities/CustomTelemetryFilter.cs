using Application.Exceptions;
using Domain.Exceptions;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Presentation.Utilities;

internal class CustomTelemetryFilter : ITelemetryProcessor
{
    private readonly ITelemetryProcessor _next;
    private readonly TelemetryClient _telemetryClient;

    public CustomTelemetryFilter(ITelemetryProcessor next, IConfiguration configuration)
    {
        _next = next;

        var telemetryConfig = TelemetryConfiguration.CreateDefault();
        var connectionString = configuration.GetSection("ApplicationInsights")["ConnectionString"];
        telemetryConfig.ConnectionString = connectionString;
        _telemetryClient = new TelemetryClient(telemetryConfig);
    }

    public void Process(ITelemetry item)
    {
        if (item is ExceptionTelemetry ex)
        {
            if (IsApplicationError(ex.Exception))
            {
                // Log app as trace (info)
                var traceTelemetry = new TraceTelemetry(ex.Exception.Message, SeverityLevel.Information);
                _telemetryClient.TrackTrace(traceTelemetry);
            }
            else
            {
                _next.Process(item);
            }
        }
    }

    private static bool IsApplicationError(Exception ex)
        => ex is TtlogixApiException || ex is DomainException || ex is ApplicationError;
}