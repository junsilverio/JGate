using JGate.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;

namespace JGate.Infrastructure.Data;

public class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<PriceList> PriceLists => Set<PriceList>();
    public DbSet<PriceListItem> PriceListItems => Set<PriceListItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.OrderNumber).IsRequired().HasMaxLength(50);
            e.Property(x => x.SubTotal).HasPrecision(18, 4);
            e.Property(x => x.TaxAmount).HasPrecision(18, 4);
            e.Property(x => x.TotalAmount).HasPrecision(18, 4);
            e.HasOne(x => x.Customer).WithMany(x => x.Orders).HasForeignKey(x => x.CustomerId);
        });

        modelBuilder.Entity<OrderItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).HasPrecision(18, 4);
            e.Property(x => x.UnitPrice).HasPrecision(18, 4);
            e.Property(x => x.Discount).HasPrecision(18, 4);
            e.Property(x => x.LineTotal).HasPrecision(18, 4);
            e.HasOne(x => x.Order).WithMany(x => x.Items).HasForeignKey(x => x.OrderId);
        });

        modelBuilder.Entity<PriceListItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.UnitPrice).HasPrecision(18, 4);
            e.Property(x => x.DiscountPercent).HasPrecision(5, 2);
            e.HasOne(x => x.PriceList).WithMany(x => x.Items).HasForeignKey(x => x.PriceListId);
        });
    }
}
