using Microsoft.EntityFrameworkCore;

namespace Notes.Api;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Note> Notes { get; set; }
}
