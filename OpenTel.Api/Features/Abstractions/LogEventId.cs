namespace OpenTel.Api.Features.Abstractions;

public static class LogEventId
{
  public static EventId ForecastEvent()
  {
    return new EventId(765, "Forecast.Api");
  }
}