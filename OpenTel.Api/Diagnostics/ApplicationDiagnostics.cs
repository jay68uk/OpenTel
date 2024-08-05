using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OpenTel.Api.Diagnostics;

public static class ApplicationDiagnostics
{
    private const string ServiceName = "OpenTel.Api";
    public const string ActivitySourceName = "Books.Features";
    
    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> BookRequestCounter = Meter.CreateCounter<long>("book.requested");
    
    
    public static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}