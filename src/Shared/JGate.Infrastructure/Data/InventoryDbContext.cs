using JGate.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace JGate.Infrastructure.Data;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.SKU).HasMaxLength(50);
            e.Property(x => x.UnitPrice).HasPrecision(18, 4);
            e.Property(x => x.MinStockLevel).HasPrecision(18, 4);
            e.HasOne(x => x.Category).WithMany(x => x.Products).HasForeignKey(x => x.CategoryId);
            e.HasOne(x => x.Supplier).WithMany(x => x.Products).HasForeignKey(x => x.SupplierId).IsRequired(false);
        });

        modelBuilder.Entity<StockLevel>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.QuantityOnHand).HasPrecision(18, 4);
            e.Property(x => x.QuantityReserved).HasPrecision(18, 4);
            e.Ignore(x => x.QuantityAvailable);
            e.HasOne(x => x.Product).WithMany(x => x.StockLevels).HasForeignKey(x => x.ProductId);
            e.HasOne(x => x.Warehouse).WithMany(x => x.StockLevels).HasForeignKey(x => x.WarehouseId);
        });

        modelBuilder.Entity<StockMovement>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).HasPrecision(18, 4);
            e.Property(x => x.UnitCost).HasPrecision(18, 4);
        });

        modelBuilder.Entity<Batch>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.LotNumber).IsRequired().HasMaxLength(100);
            e.Property(x => x.Quantity).HasPrecision(18, 4);
            e.Property(x => x.RemainingQuantity).HasPrecision(18, 4);
        });
    }
}
