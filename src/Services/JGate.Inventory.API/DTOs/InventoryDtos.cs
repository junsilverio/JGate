namespace JGate.Inventory.API.DTOs;

public record CategoryDto(Guid Id, string Name, string? Description);
public record CreateCategoryRequest(string Name, string? Description);

public record SupplierDto(Guid Id, string Name, string? ContactPerson, string? Email, string? Phone, string? Address, bool IsActive);
public record CreateSupplierRequest(string Name, string? ContactPerson, string? Email, string? Phone, string? Address);

public record WarehouseDto(Guid Id, string Name, string Location, string? Zone, decimal? MinTemperature, decimal? MaxTemperature);
public record CreateWarehouseRequest(string Name, string Location, string? Zone, decimal? MinTemperature, decimal? MaxTemperature);

public record ProductDto(Guid Id, string Name, string? SKU, string? Description, Guid CategoryId, string CategoryName, Guid? SupplierId, string? SupplierName, decimal UnitPrice, string Unit, decimal MinStockLevel, bool IsActive, bool RequiresColdStorage);
public record CreateProductRequest(string Name, string? SKU, string? Description, Guid CategoryId, Guid? SupplierId, decimal UnitPrice, string Unit, decimal MinStockLevel, bool RequiresColdStorage);
public record UpdateProductRequest(string Name, string? SKU, string? Description, Guid CategoryId, Guid? SupplierId, decimal UnitPrice, string Unit, decimal MinStockLevel, bool IsActive, bool RequiresColdStorage);

public record StockLevelDto(Guid Id, Guid ProductId, string ProductName, Guid WarehouseId, string WarehouseName, decimal QuantityOnHand, decimal QuantityReserved, decimal QuantityAvailable);

public record StockMovementDto(Guid Id, Guid ProductId, string ProductName, string MovementType, decimal Quantity, decimal? UnitCost, string? ReferenceNumber, string? Notes, DateTime MovementDate);
public record CreateStockMovementRequest(Guid ProductId, Guid? WarehouseId, Guid? BatchId, string MovementType, decimal Quantity, decimal? UnitCost, string? ReferenceNumber, string? Notes);

public record BatchDto(Guid Id, string LotNumber, Guid ProductId, string ProductName, DateTime ManufactureDate, DateTime ExpiryDate, decimal Quantity, decimal RemainingQuantity, Guid? WarehouseId);
public record CreateBatchRequest(string LotNumber, Guid ProductId, DateTime ManufactureDate, DateTime ExpiryDate, decimal Quantity, Guid? WarehouseId, string? Notes);
