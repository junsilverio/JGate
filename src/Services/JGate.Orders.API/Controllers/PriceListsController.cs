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
public class PriceListsController(OrdersDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PriceListDto>>> GetAll()
    {
        var items = await db.PriceLists.Where(p => !p.IsDeleted)
            .Select(p => new PriceListDto(p.Id, p.Name, p.ValidFrom, p.ValidTo, p.IsActive))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePriceListRequest request)
    {
        var pl = new PriceList { Name = request.Name, ValidFrom = request.ValidFrom, ValidTo = request.ValidTo };
        db.PriceLists.Add(pl);
        await db.SaveChangesAsync();
        return Ok(new { id = pl.Id });
    }
}
