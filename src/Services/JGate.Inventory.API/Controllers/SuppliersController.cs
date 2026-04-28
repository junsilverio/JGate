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
public class SuppliersController(InventoryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var items = await db.Suppliers.Where(s => !s.IsDeleted)
            .Select(s => new SupplierDto(s.Id, s.Name, s.ContactPerson, s.Email, s.Phone, s.Address, s.IsActive))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierRequest request)
    {
        var supplier = new Supplier { Name = request.Name, ContactPerson = request.ContactPerson, Email = request.Email, Phone = request.Phone, Address = request.Address };
        db.Suppliers.Add(supplier);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = supplier.Id }, new { id = supplier.Id });
    }
}
