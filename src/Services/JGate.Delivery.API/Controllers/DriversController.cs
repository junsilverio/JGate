using JGate.Delivery.API.DTOs;
using JGate.Domain.Entities.Delivery;
using JGate.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Delivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DriversController(DeliveryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DriverDto>>> GetAll()
    {
        var items = await db.Drivers.Where(d => !d.IsDeleted)
            .Select(d => new DriverDto(d.Id, d.FirstName, d.LastName, d.Email, d.Phone, d.LicenseNumber, d.IsAvailable, d.IsActive))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDriverRequest request)
    {
        var driver = new Driver { FirstName = request.FirstName, LastName = request.LastName, Email = request.Email, Phone = request.Phone, LicenseNumber = request.LicenseNumber };
        db.Drivers.Add(driver);
        await db.SaveChangesAsync();
        return Ok(new { id = driver.Id });
    }

    [HttpPatch("{id:guid}/availability")]
    public async Task<IActionResult> SetAvailability(Guid id, [FromBody] bool isAvailable)
    {
        var driver = await db.Drivers.FindAsync(id);
        if (driver == null || driver.IsDeleted) return NotFound();
        driver.IsAvailable = isAvailable;
        driver.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
