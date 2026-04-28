using JGate.Domain.Entities.Delivery;
using Microsoft.EntityFrameworkCore;

namespace JGate.Infrastructure.Data;

public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
{
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<DeliveryRoute> Routes => Set<DeliveryRoute>();
    public DbSet<DeliveryStop> Stops => Set<DeliveryStop>();
    public DbSet<DeliveryOrder> DeliveryOrders => Set<DeliveryOrder>();
    public DbSet<TemperatureLog> TemperatureLogs => Set<TemperatureLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Driver>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            e.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            e.Ignore(x => x.FullName);
        });

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.PlateNumber).IsRequired().HasMaxLength(20);
            e.Property(x => x.MaxLoadKg).HasPrecision(10, 2);
        });

        modelBuilder.Entity<DeliveryRoute>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.RouteCode).IsRequired().HasMaxLength(50);
            e.HasOne(x => x.Driver).WithMany(x => x.Routes).HasForeignKey(x => x.DriverId).IsRequired(false);
            e.HasOne(x => x.Vehicle).WithMany(x => x.Routes).HasForeignKey(x => x.VehicleId).IsRequired(false);
        });

        modelBuilder.Entity<DeliveryStop>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Route).WithMany(x => x.Stops).HasForeignKey(x => x.RouteId);
        });

        modelBuilder.Entity<DeliveryOrder>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Stop).WithMany(x => x.DeliveryOrders).HasForeignKey(x => x.StopId).IsRequired(false);
        });

        modelBuilder.Entity<TemperatureLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Temperature).HasPrecision(6, 2);
            e.HasOne(x => x.DeliveryOrder).WithMany(x => x.TemperatureLogs).HasForeignKey(x => x.DeliveryOrderId);
        });
    }
}
