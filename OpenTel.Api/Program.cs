using System.Diagnostics;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using OpenTel.Api;
using OpenTel.Api.Diagnostics;
using OpenTel.Api.Extensions;
using OpenTel.Book.Contracts;
using Serilog;

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
  
  builder.Logging.AddConsole();
  builder.Services.AddFastEndpoints();
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();

  builder.Services.AddDbContext<BooksDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("BooksDb")));

  builder.AddOpenTelemetry();

  builder.AddRabbitMq();

  var app = builder.Build();

  EnsureDbCreated(app);

  if (app.Environment.IsDevelopment())
  {
    app.UseSwagger();
    app.UseSwaggerUI();
  }

  app.UseHttpsRedirection();
  app.UseFastEndpoints();
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

static void EnsureDbCreated(WebApplication app)
{
  using var scope = app.Services.CreateScope();
  var scopedServices = scope.ServiceProvider;
  var context = scopedServices.GetRequiredService<BooksDbContext>();
  if (context.Database.CanConnect() is false)
  {
    Task.Delay(5000);
  }
  
  context.Database.EnsureCreated();

  try
  {
    context.Books.Add(new Book(){ Author = "J.R.R Tolkien", Id = Guid.Parse("D7FFFD73-2B93-45C3-BF6B-4E5235993FD2"), Title = "The Hobbit" });
    context.SaveChanges();
  }
  catch (Exception e)
  {
    Console.WriteLine("Test book already added");
  }
}