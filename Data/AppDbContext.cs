/*
    File: AppDbContext.cs
    Summary: Entity Framework Core database context for the TechHive API. 
    Manages the Users DbSet and configures model properties, including unique constraints for user emails.
*/
using Microsoft.EntityFrameworkCore;
using TechHiveAPI.Models;

namespace TechHiveAPI.Data;

/// <summary>
/// Database context for the TechHive API.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enforce unique email constraint
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
