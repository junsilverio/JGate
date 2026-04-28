using JGate.Domain.Entities.Inventory;
using JGate.Domain.Enums;
using JGate.Infrastructure.Data;
using JGate.Inventory.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Inventory.API.Controllers;

[ApiController]
[Route("api")]
[Authorize]
public class StockController(InventoryDbContext db) : ControllerBase
{
    [HttpGet("stock")]
    public async Task<ActionResult<IEnumerable<StockLevelDto>>> GetStockLevels()
    {
        var items = await db.StockLevels
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Select(s => new StockLevelDto(s.Id, s.ProductId, s.Product!.Name, s.WarehouseId, s.Warehouse!.Name,
                s.QuantityOnHand, s.QuantityReserved, s.QuantityOnHand - s.QuantityReserved))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("stock/low")]
    public async Task<ActionResult<IEnumerable<StockLevelDto>>> GetLowStock()
    {
        var items = await db.StockLevels
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Where(s => (s.QuantityOnHand - s.QuantityReserved) < s.Product!.MinStockLevel)
            .Select(s => new StockLevelDto(s.Id, s.ProductId, s.Product!.Name, s.WarehouseId, s.Warehouse!.Name,
                s.QuantityOnHand, s.QuantityReserved, s.QuantityOnHand - s.QuantityReserved))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("stock-movements")]
    public async Task<ActionResult<IEnumerable<StockMovementDto>>> GetMovements()
    {
        var items = await db.StockMovements
            .Include(m => m.Product)
            .OrderByDescending(m => m.MovementDate)
            .Select(m => new StockMovementDto(m.Id, m.ProductId, m.Product!.Name, m.MovementType.ToString(),
                m.Quantity, m.UnitCost, m.ReferenceNumber, m.Notes, m.MovementDate))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("stock-movements")]
    public async Task<IActionResult> CreateMovement([FromBody] CreateStockMovementRequest request)
    {
        if (!Enum.TryParse<StockMovementType>(request.MovementType, out var movementType))
            return BadRequest(new { message = "Invalid movement type." });

        if (request.WarehouseId == null)
            return BadRequest(new { message = "WarehouseId is required for stock movements." });

        var movement = new StockMovement
        {
            ProductId = request.ProductId, WarehouseId = request.WarehouseId,
            BatchId = request.BatchId, MovementType = movementType,
            Quantity = request.Quantity, UnitCost = request.UnitCost,
            ReferenceNumber = request.ReferenceNumber, Notes = request.Notes
        };
        db.StockMovements.Add(movement);

        // Receipt and Return increase stock; all other types decrease it
        bool isIncrease = movementType is StockMovementType.Receipt or StockMovementType.Return;

        var stockLevel = await db.StockLevels.FirstOrDefaultAsync(s =>
            s.ProductId == request.ProductId && s.WarehouseId == request.WarehouseId);
        if (stockLevel == null)
        {
            stockLevel = new StockLevel { ProductId = request.ProductId, WarehouseId = request.WarehouseId.Value };
            db.StockLevels.Add(stockLevel);
        }
        stockLevel.QuantityOnHand += isIncrease ? request.Quantity : -request.Quantity;
        stockLevel.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok(new { id = movement.Id });
    }

    [HttpGet("batches")]
    public async Task<ActionResult<IEnumerable<BatchDto>>> GetBatches()
    {
        var items = await db.Batches
            .Include(b => b.Product)
            .Select(b => new BatchDto(b.Id, b.LotNumber, b.ProductId, b.Product!.Name,
                b.ManufactureDate, b.ExpiryDate, b.Quantity, b.RemainingQuantity, b.WarehouseId))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("batches")]
    public async Task<IActionResult> CreateBatch([FromBody] CreateBatchRequest request)
    {
        var batch = new Batch
        {
            LotNumber = request.LotNumber, ProductId = request.ProductId,
            ManufactureDate = request.ManufactureDate, ExpiryDate = request.ExpiryDate,
            Quantity = request.Quantity, RemainingQuantity = request.Quantity,
            WarehouseId = request.WarehouseId, Notes = request.Notes
        };
        db.Batches.Add(batch);
        await db.SaveChangesAsync();
        return Ok(new { id = batch.Id });
    }

    [HttpGet("warehouses")]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses()
    {
        var items = await db.Warehouses.Where(w => !w.IsDeleted)
            .Select(w => new WarehouseDto(w.Id, w.Name, w.Location, w.Zone, w.MinTemperature, w.MaxTemperature))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost("warehouses")]
    public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseRequest request)
    {
        var warehouse = new Warehouse { Name = request.Name, Location = request.Location, Zone = request.Zone, MinTemperature = request.MinTemperature, MaxTemperature = request.MaxTemperature };
        db.Warehouses.Add(warehouse);
        await db.SaveChangesAsync();
        return Ok(new { id = warehouse.Id });
    }
}
