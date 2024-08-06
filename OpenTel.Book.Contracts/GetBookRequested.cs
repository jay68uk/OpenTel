namespace OpenTel.Book.Contracts;

public record CreateClientRequested(Client Client, DateTimeOffset RequestedOn);