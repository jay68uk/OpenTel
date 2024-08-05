using System.Reflection;
using Npgsql;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTel.Api.Diagnostics;

public static class OpenTelemetryConfigurationExtensions
{
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        const string serviceName = "WeatherForecast.Api";
        
        var otlpEndpoint = new Uri(builder.Configuration.GetValue<string>("OTLP_Endpoint")!);
        Console.WriteLine($"OTLP: {otlpEndpoint.AbsoluteUri}");

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource
                    .AddService(serviceName,
                        "WeatherForecast.OpenTelemetry",
                        Assembly.GetExecutingAssembly().GetName().Version!.ToString()) 
                    // service.instance.id is auto-generated but can be set to a cloud service ID
                    .AddAttributes(new[]
                    {
                        new KeyValuePair<string, object>("service.otherAttribute",
                            "Ensure any value is cast to a primitive") 
                    });
            })
            .WithTracing(tracing =>
                    tracing
                        .AddAspNetCoreInstrumentation() // instrumentation library for http requests, use system.diagnostics
                        .AddHttpClientInstrumentation()
                        .AddSource(ApplicationDiagnostics.ActivitySourceName)
                        //.AddNpgsql()
                        // .AddSource(RabbitMqDiagnostics.ActivitySourceName)
                        .AddConsoleExporter() // use this when starting out 
                        //.AddOtlpExporter(options => options.Endpoint = new Uri("http://jaeger:4317"))
                        .AddOtlpExporter(options =>
                            options.Endpoint = new Uri("http://otel-collector:4317"))
            )
            .WithMetrics(metrics =>
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter(ApplicationDiagnostics.Meter.Name)
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                        options.Endpoint = new Uri("http://otel-collector:4317"))
            )
            .WithLogging(
                logging=>
                    logging
                        .AddConsoleExporter()
                        .AddOtlpExporter(options => 
                            options.Endpoint = new Uri("http://otel-collector:4317"))
            );

        return builder;
    }
}