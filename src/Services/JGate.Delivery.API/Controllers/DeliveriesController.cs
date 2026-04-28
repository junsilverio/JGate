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
public class DeliveriesController(DeliveryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeliveryOrderDto>>> GetAll([FromQuery] string? status = null)
    {
        var query = db.DeliveryOrders.Where(d => !d.IsDeleted);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<DeliveryStatus>(status, out var s))
            query = query.Where(d => d.Status == s);
        var items = await query.Select(d => new DeliveryOrderDto(d.Id, d.StopId, d.OrderId, d.OrderNumber,
            d.Status.ToString(), d.DeliveredAt, d.ProofOfDelivery, d.FailureReason)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDeliveryOrderRequest request)
    {
        var delivery = new DeliveryOrder { StopId = request.StopId, OrderId = request.OrderId, OrderNumber = request.OrderNumber };
        db.DeliveryOrders.Add(delivery);
        await db.SaveChangesAsync();
        return Ok(new { id = delivery.Id });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateDeliveryStatusRequest request)
    {
        var delivery = await db.DeliveryOrders.FindAsync(id);
        if (delivery == null || delivery.IsDeleted) return NotFound();
        if (!Enum.TryParse<DeliveryStatus>(request.Status, out var newStatus))
            return BadRequest(new { message = "Invalid status." });
        delivery.Status = newStatus;
        if (newStatus == DeliveryStatus.Delivered) delivery.DeliveredAt = DateTime.UtcNow;
        delivery.ProofOfDelivery = request.ProofOfDelivery;
        delivery.FailureReason = request.FailureReason;
        delivery.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("temperature-logs")]
    public async Task<IActionResult> LogTemperature([FromBody] CreateTemperatureLogRequest request)
    {
        var log = new TemperatureLog { DeliveryOrderId = request.DeliveryOrderId, Temperature = request.Temperature, Notes = request.Notes };
        db.TemperatureLogs.Add(log);
        await db.SaveChangesAsync();
        return Ok(new { id = log.Id });
    }
}
