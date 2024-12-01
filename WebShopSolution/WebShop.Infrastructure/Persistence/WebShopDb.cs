using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entities;

namespace WebShop.Infrastructure.Persistence;

public class WebShopDb : DbContext
{
    public WebShopDb() : base(new DbContextOptions<WebShopDb>())
    {

    }

    public WebShopDb(DbContextOptions<WebShopDb> options) : base(options)
    {

    }

    public DbSet<ProductEntity> Products { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });
    }
}