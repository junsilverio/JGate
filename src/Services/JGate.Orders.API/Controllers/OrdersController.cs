using JGate.Domain.Entities.Orders;
using JGate.Domain.Enums;
using JGate.Infrastructure.Data;
using JGate.Orders.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController(OrdersDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll([FromQuery] string? status = null)
    {
        var query = db.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .Where(o => !o.IsDeleted);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var s))
            query = query.Where(o => o.Status == s);

        var orders = await query.OrderByDescending(o => o.OrderDate)
            .Select(o => ToDto(o)).ToListAsync();
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var o = await db.Orders.Include(x => x.Customer).Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
        if (o == null) return NotFound();
        return Ok(ToDto(o));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
    {
        var order = new Order
        {
            OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..4].ToUpper()}",
            CustomerId = request.CustomerId,
            RequiredDate = request.RequiredDate,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            Status = OrderStatus.Draft
        };

        foreach (var item in request.Items)
        {
            var lineTotal = (item.Quantity * item.UnitPrice) - item.Discount;
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId, ProductName = item.ProductName,
                Quantity = item.Quantity, Unit = item.Unit,
                UnitPrice = item.UnitPrice, Discount = item.Discount, LineTotal = lineTotal
            });
        }

        order.SubTotal = order.Items.Sum(i => i.LineTotal);
        order.TaxAmount = order.SubTotal * 0.12m;
        order.TotalAmount = order.SubTotal + order.TaxAmount;

        db.Orders.Add(order);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, new { id = order.Id, orderNumber = order.OrderNumber });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var order = await db.Orders.FindAsync(id);
        if (order == null || order.IsDeleted) return NotFound();
        if (!Enum.TryParse<OrderStatus>(request.Status, out var newStatus))
            return BadRequest(new { message = "Invalid status." });
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var order = await db.Orders.FindAsync(id);
        if (order == null || order.IsDeleted) return NotFound();
        order.Status = OrderStatus.Cancelled;
        order.IsDeleted = true;
        order.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static OrderDto ToDto(Order o) => new(
        o.Id, o.OrderNumber, o.CustomerId, o.Customer?.Name ?? string.Empty,
        o.Status.ToString(), o.OrderDate, o.RequiredDate, o.ShippingAddress, o.Notes,
        o.SubTotal, o.TaxAmount, o.TotalAmount,
        o.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.Quantity, i.Unit, i.UnitPrice, i.Discount, i.LineTotal)));
}
