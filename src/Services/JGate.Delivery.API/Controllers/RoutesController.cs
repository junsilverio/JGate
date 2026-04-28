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
public class RoutesController(DeliveryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryRouteDto>>> GetAll()
    {
        var items = await db.Routes
            .Include(r => r.Driver)
            .Include(r => r.Vehicle)
            .Where(r => !r.IsDeleted)
            .Select(r => new DeliveryRouteDto(r.Id, r.RouteCode, r.Name, r.PlannedDate,
                r.DriverId, r.Driver != null ? $"{r.Driver.FirstName} {r.Driver.LastName}" : null,
                r.VehicleId, r.Vehicle != null ? r.Vehicle.PlateNumber : null, r.Notes))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRouteRequest request)
    {
        var route = new DeliveryRoute
        {
            RouteCode = $"RT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            Name = request.Name, PlannedDate = request.PlannedDate,
            DriverId = request.DriverId, VehicleId = request.VehicleId, Notes = request.Notes
        };
        db.Routes.Add(route);
        await db.SaveChangesAsync();
        return Ok(new { id = route.Id, routeCode = route.RouteCode });
    }
}
