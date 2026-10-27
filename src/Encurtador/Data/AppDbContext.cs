using System;
using Encurtador.Models;
using Microsoft.EntityFrameworkCore;
namespace Encurtador.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Link> Links => Set<Link>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Link>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Codigo).HasMaxLength(10).IsRequired();
            e.HasIndex(l => l.Codigo).IsUnique();
            e.Property(l => l.UrlOriginal).HasMaxLength(2048).IsRequired();
            e.Property(l => l.CriadoEm).IsRequired();
        });
    }
}