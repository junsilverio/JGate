using JGate.Domain.Entities.Inventory;
using JGate.Infrastructure.Data;
using JGate.Inventory.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController(InventoryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => !p.IsDeleted)
            .Select(p => new ProductDto(p.Id, p.Name, p.SKU, p.Description, p.CategoryId,
                p.Category!.Name, p.SupplierId, p.Supplier != null ? p.Supplier.Name : null,
                p.UnitPrice, p.Unit, p.MinStockLevel, p.IsActive, p.RequiresColdStorage))
            .ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var p = await db.Products.Include(x => x.Category).Include(x => x.Supplier)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (p == null) return NotFound();
        return Ok(new ProductDto(p.Id, p.Name, p.SKU, p.Description, p.CategoryId,
            p.Category!.Name, p.SupplierId, p.Supplier?.Name, p.UnitPrice, p.Unit,
            p.MinStockLevel, p.IsActive, p.RequiresColdStorage));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request)
    {
        var product = new Product
        {
            Name = request.Name, SKU = request.SKU, Description = request.Description,
            CategoryId = request.CategoryId, SupplierId = request.SupplierId,
            UnitPrice = request.UnitPrice, Unit = request.Unit,
            MinStockLevel = request.MinStockLevel, RequiresColdStorage = request.RequiresColdStorage
        };
        db.Products.Add(product);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, new { id = product.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var product = await db.Products.FindAsync(id);
        if (product == null || product.IsDeleted) return NotFound();
        product.Name = request.Name; product.SKU = request.SKU; product.Description = request.Description;
        product.CategoryId = request.CategoryId; product.SupplierId = request.SupplierId;
        product.UnitPrice = request.UnitPrice; product.Unit = request.Unit;
        product.MinStockLevel = request.MinStockLevel; product.IsActive = request.IsActive;
        product.RequiresColdStorage = request.RequiresColdStorage; product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var product = await db.Products.FindAsync(id);
        if (product == null || product.IsDeleted) return NotFound();
        product.IsDeleted = true; product.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
