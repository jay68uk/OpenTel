using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OpenTel.Api.Diagnostics;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "WeatherForecast.Api";
    public const string ActivitySourceName = "WeatherForecast.Api";
    
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> ForecastRequestCounter = Meter.CreateCounter<long>("forecast.requested");
    
    
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}