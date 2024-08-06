using System.Diagnostics;
using FastEndpoints;
using OpenTel.Api.Diagnostics;
using OpenTel.Api.Features.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace OpenTel.Api.Features;

public record BookResponse(Guid Id, string Title, string Author);


public class GetBookById : EndpointWithoutRequest<BookResponse>
{
  private readonly ILogger<GetBookById> _logger;
  private readonly BooksDbContext _dbContext;

  public GetBookById(ILogger<GetBookById> logger, BooksDbContext dbContext)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public override void Configure()
  {
    Get("/book/{Id}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var bookId = Route<Guid>("Id");
    
    Activity.Current?.SetTag("book.id", bookId.ToString() + "created using Activity.Current?.SetTag(...), Ensure any value used here is cast to a primitive");
    
    try
    {
      if (bookId == Guid.Parse("D7FFFD73-2B93-45C3-BF6B-4E5235993FD0"))
      {
        throw new Exception("Get book test exception");
      }
    
      //Activity.Current?.SetTag("book.id", bookId.ToString() + "created using Activity.Current?.SetTag(...), Ensure any value used here is cast to a primitive");
      
      ApplicationDiagnostics.BookRequestCounter.Add(1,
        new[] { new KeyValuePair<string, object?>("books.requested", bookId.ToString()) });

      //Baggage.SetBaggage("book.id", bookId.ToString());
      
      var result = _dbContext.Books.SingleOrDefault(b => b.Id == bookId);

      if (result is null)
      {
        _logger.LogInformation(LogEventId.BookEvent(), "Book {bookId} was not found!", bookId);

        Activity.Current?.AddEvent(
          new ActivityEvent(
            "getbookbyid.handler",
            tags: new ActivityTagsCollection(new KeyValuePair<string, object?>[]
              {
                new("book.id", bookId.ToString()),
                new("book.status", "404 not found")
              }
            )
          )
        );

        await SendNotFoundAsync();
        return;
      }


      using var activity = ApplicationDiagnostics.ActivitySource
      .StartActivity("book.retrieved", ActivityKind.Server);
      activity?.SetTag("book.id", bookId.ToString());

      await SendOkAsync(new BookResponse(result.Id, result.Title, result.Author));
    }
    catch (Exception e)
    {
      Activity.Current?.SetStatus(Status.Error);
      Activity.Current?.RecordException(e);
      throw;
    }
  }
}

