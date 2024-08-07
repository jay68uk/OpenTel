# OpenTelemetry Basic Features Example

## Running The Demo
- 'docker compose up --build'
- In the OpenTel.Api project open OpenTel.Api.Http
- When sending requests to the docker container use @OpenTel.Api_HostAddress = http://localhost:8080
- When debugging and using the docker container services (db and telemetry receivers) use @OpenTel.Api_HostAddress = http://localhost:5240
- Run the GET requests to generate telemetry
  
## Telemetry Endpoints
After generating some telemetry it can be visualised using the following

### Traces
- [Jaeger](http://localhost:16686)

### Metrics
- [Prometheus](http://localhost:9090)

- On the top right, next to the 'Execute' button, click the 'Open Metrics Explorer'
- Select a metric from the list and then click 'Execute'  

### Visual and drill down
- [Grafana](http://localhost:3000)

- On the left menu select 'Dashboards'
- Click on 'Create Dashboard'
- Click on 'Import Dashboard'
- In the 'Load' text box enter 19924 for the dashboard Id
- In the bottom dropdown select 'Prometheus'
- Click 'Import'
- Repeat the above steps but use 19925 for the dashboard Id

## Handy Resources:

- [OpenTelemetry](https://opentelemetry.io/)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [OpenTelemetry Auto Instrumentation](https://opentelemetry.io/docs/zero-code/)
- [OpenTelemetry Collector](https://opentelemetry.io/docs/collector/)
- [OpenTelemetry Collector Contrib](https://github.com/open-telemetry/opentelemetry-collector-contrib)
- [.NET observability with OpenTelemetry](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/observability-with-otel)

