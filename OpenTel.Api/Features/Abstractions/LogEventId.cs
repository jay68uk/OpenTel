namespace OpenTel.Api.Features.Abstractions;

public static class LogEventId
{
  public static EventId BookEvent()
  {
    return new EventId(765);
  }
}