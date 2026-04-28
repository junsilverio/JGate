using JGate.Delivery.API.DTOs;
using JGate.Domain.Entities.Delivery;
using JGate.Domain.Enums;
using JGate.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Delivery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController(DeliveryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleDto>>> GetAll()
    {
        var items = await db.Vehicles.Where(v => !v.IsDeleted)
            .Select(v => new VehicleDto(v.Id, v.PlateNumber, v.Make, v.Model, v.Year, v.Type.ToString(), v.MaxLoadKg, v.HasTemperatureControl, v.IsAvailable, v.IsActive))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
    {
        if (!Enum.TryParse<VehicleType>(request.Type, out var vehicleType))
            return BadRequest(new { message = "Invalid vehicle type." });
        var vehicle = new Vehicle { PlateNumber = request.PlateNumber, Make = request.Make, Model = request.Model, Year = request.Year, Type = vehicleType, MaxLoadKg = request.MaxLoadKg, HasTemperatureControl = request.HasTemperatureControl };
        db.Vehicles.Add(vehicle);
        await db.SaveChangesAsync();
        return Ok(new { id = vehicle.Id });
    }
}
