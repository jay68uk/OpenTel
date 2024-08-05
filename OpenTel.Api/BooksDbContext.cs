
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OpenTel.Api;

public class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
}

public class Book
{
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; }
    public string Author { get; init; }
}