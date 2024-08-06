namespace OpenTel.Book.Contracts;

public record GetBookRequested(Book Book, DateTimeOffset RequestedOn);