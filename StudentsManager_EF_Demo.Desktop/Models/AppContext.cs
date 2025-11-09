using Microsoft.EntityFrameworkCore;

namespace StudentsManager_EF_Demo.Desktop.Models;

public class AppContext : DbContext
{
    private const string ConnectionString = "Host=localhost;Port=5432;Database=students;Username=postgres;Password=1234";
    public DbSet<Student> Students { get; set; }

    public AppContext() : base()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(ConnectionString);
    }
}