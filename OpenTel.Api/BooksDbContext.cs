using Microsoft.EntityFrameworkCore;

namespace OpenTel.Api;

public class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book.Contracts.Book> Books { get; set; }
}