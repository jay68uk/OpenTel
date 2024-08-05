using System.Diagnostics;
using FastEndpoints;
using OpenTel.Api.Diagnostics;
using OpenTel.Api.Features.Abstractions;

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
    ApplicationDiagnostics.BookRequestCounter.Add(1,
      new[] { new KeyValuePair<string, object?>("books.requested", bookId.ToString()) });

    var result = _dbContext.Books.SingleOrDefault(b => b.Id == bookId);

    if (result is null)
    {
      _logger.LogInformation(LogEventId.BookEvent(), "Book {bookId} was not found!", bookId);
      await SendNotFoundAsync();
      return;
    }

    using var activity = ApplicationDiagnostics.ActivitySource
      .StartActivity("book.retrieved", ActivityKind.Server);

    await SendOkAsync(new BookResponse(result.Id, result.Title, result.Author));
  }
}

