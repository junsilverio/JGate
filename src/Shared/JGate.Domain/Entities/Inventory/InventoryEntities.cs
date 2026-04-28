using JGate.Domain.Common;

namespace JGate.Domain.Entities.Inventory;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Supplier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Product> Products { get; set; } = new List<Product>();
}

public class Warehouse : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public decimal? MinTemperature { get; set; }
    public decimal? MaxTemperature { get; set; }
    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
}

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public decimal UnitPrice { get; set; }
    public string Unit { get; set; } = "kg";
    public decimal MinStockLevel { get; set; }
    public bool IsActive { get; set; } = true;
    public bool RequiresColdStorage { get; set; }
    public ICollection<StockLevel> StockLevels { get; set; } = new List<StockLevel>();
    public ICollection<Batch> Batches { get; set; } = new List<Batch>();
}

public class Batch : BaseEntity
{
    public string LotNumber { get; set; } = string.Empty;
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime ManufactureDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public string? Notes { get; set; }
}

public class StockLevel : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; set; }
    public decimal QuantityAvailable => QuantityOnHand - QuantityReserved;
}

public class StockMovement : BaseEntity
{
    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    public Guid? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public Guid? BatchId { get; set; }
    public Batch? Batch { get; set; }
    public Enums.StockMovementType MovementType { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public DateTime MovementDate { get; set; } = DateTime.UtcNow;
}
