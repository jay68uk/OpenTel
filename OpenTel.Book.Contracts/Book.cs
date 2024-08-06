using System.ComponentModel.DataAnnotations;

namespace OpenTel.Api;

public class Book
{
  [Key]
  public Guid Id { get; init; } = Guid.NewGuid();
  public string Title { get; init; }
  public string Author { get; init; }
}