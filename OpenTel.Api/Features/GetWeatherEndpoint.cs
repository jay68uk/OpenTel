using System.Diagnostics;
using FastEndpoints;
using OpenTel.Api.Diagnostics;
using OpenTel.Api.Features.Abstractions;

namespace OpenTel.Api.Features;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
  public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public class GetWeatherEndpoint : EndpointWithoutRequest<WeatherForecast[]>
{
  private readonly ILogger<GetWeatherEndpoint> _logger;

  private static string[] _summaries = new[]
  {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
  };

  public GetWeatherEndpoint(ILogger<GetWeatherEndpoint> logger)
  {
    _logger = logger;
  }

  public override void Configure()
  {
    Get("/weatherforecast");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
          DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
          Random.Shared.Next(-20, 55),
          _summaries[Random.Shared.Next(_summaries.Length)]
        ))
      .ToArray();

    ApplicationDiagnostics.ForecastRequestCounter.Add(1,
      new[] { new KeyValuePair<string, object?>("forecast.summary", forecast[0].Summary) });

    _logger.LogInformation(LogEventId.ForecastEvent(), "Forecast created successfully");

    using var activity = ApplicationDiagnostics.ActivitySource
      .StartActivity("Forecast Created", ActivityKind.Server);

    await SendOkAsync(forecast);
  }
}

