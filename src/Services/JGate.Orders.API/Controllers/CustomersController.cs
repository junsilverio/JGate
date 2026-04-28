using JGate.Domain.Entities.Orders;
using JGate.Infrastructure.Data;
using JGate.Orders.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Orders.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController(OrdersDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAll()
    {
        var items = await db.Customers.Where(c => !c.IsDeleted)
            .Select(c => new CustomerDto(c.Id, c.Name, c.ContactPerson, c.Email, c.Phone, c.Address, c.TaxId, c.IsActive))
            .ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CustomerDto>> GetById(Guid id)
    {
        var c = await db.Customers.FindAsync(id);
        if (c == null || c.IsDeleted) return NotFound();
        return Ok(new CustomerDto(c.Id, c.Name, c.ContactPerson, c.Email, c.Phone, c.Address, c.TaxId, c.IsActive));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = new Customer { Name = request.Name, ContactPerson = request.ContactPerson, Email = request.Email, Phone = request.Phone, Address = request.Address, TaxId = request.TaxId };
        db.Customers.Add(customer);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, new { id = customer.Id });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var customer = await db.Customers.FindAsync(id);
        if (customer == null || customer.IsDeleted) return NotFound();
        customer.Name = request.Name; customer.ContactPerson = request.ContactPerson; customer.Email = request.Email;
        customer.Phone = request.Phone; customer.Address = request.Address; customer.TaxId = request.TaxId;
        customer.IsActive = request.IsActive; customer.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }
}
