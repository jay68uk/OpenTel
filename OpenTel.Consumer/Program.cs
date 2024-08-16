using OpenTel.Book.Contracts;
using OpenTel.Consumer.Diagnostics;
using OpenTel.RabbitMQ;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.OpenTelemetry;

var startupLogger = Log.Logger = new LoggerConfiguration()
  .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
  .Enrich.FromLogContext()
  .WriteTo.OpenTelemetry() //defaults to http://localhost:4317 and GRPC protocol
  .WriteTo.Console()
  .CreateBootstrapLogger();

try
{

  var builder = WebApplication.CreateBuilder(args);

  builder.Host.UseSerilog((_, configureLogger) => 
    configureLogger.ReadFrom.Configuration(builder.Configuration));
  
  builder.Services.AddScoped<BookEventHandler>();
  builder.Services.AddScoped<RabbitMqConsumer<GetBookRequested>>(
    sp => new RabbitMqConsumer<GetBookRequested>
    (builder.Configuration.GetConnectionString("RabbitMq")!,
      sp.GetRequiredService<BookEventHandler>()));

  builder.AddOpenTelemetry();

  var app = builder.Build();

  app.MapGet("/", () => "Consumer");

  using var scope = app.Services.CreateScope();
  var rabbitMqConsumer = scope.ServiceProvider.GetRequiredService<RabbitMqConsumer<GetBookRequested>>();
  rabbitMqConsumer.StartConsuming("book.events", "consumer.get_book_event");

  app.Run();
}
catch (Exception e)
{
  Log.Fatal(e, "Error registering or starting services!");
}
finally
{
  await Log.CloseAndFlushAsync();
}


public class BookEventHandler : IEventHandler<GetBookRequested>
{
  private readonly ILogger<BookEventHandler> _logger;

  public BookEventHandler(ILogger<BookEventHandler> logger)
  {
    _logger = logger;
  }
  public Task HandleAsync(GetBookRequested @event)
  {
    _logger.LogInformation("Sending notification about book {BookId} {BookTitle}", @event!.Book.Id,
      @event!.Book.Title);
    return Task.CompletedTask;
  }
}