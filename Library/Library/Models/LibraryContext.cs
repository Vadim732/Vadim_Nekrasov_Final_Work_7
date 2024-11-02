using Microsoft.EntityFrameworkCore;

namespace Library.Models;

public class LibraryContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) {}
}