using Microsoft.EntityFrameworkCore;
using UrlShortener.Web.Models;
using UrlShortener.Web.Models.Domain;

namespace UrlShortener.Web.Data;

public class UrlShortenerDbContext: DbContext
{
    public UrlShortenerDbContext(DbContextOptions<UrlShortenerDbContext> options) : base(options)
    {
        
    }
    
    public DbSet<UrlMapping> UrlMappings { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<AboutContent> AboutContents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e=>e.Username).IsUnique();
            entity.Property(e => e.Role).HasConversion<int>();
            
            entity.HasMany(e=>e.CreatedUrls)
                .WithOne(e=>e.CreatedBy)
                .HasForeignKey(e=>e.CreatedById)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UrlMapping>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e=>e.ShortCode).IsUnique();
            entity.HasIndex(e=>e.OriginalUrl).IsUnique();

            entity.Property(e => e.ShortCode).IsRequired();
            entity.Property(e => e.OriginalUrl).IsRequired();
        });

        modelBuilder.Entity<AboutContent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.Title).IsRequired();
            
        });
    }
}