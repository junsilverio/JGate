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
public class CategoriesController(InventoryDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var items = await db.Categories.Where(c => !c.IsDeleted)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description)).ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var category = new Category { Name = request.Name, Description = request.Description };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new { id = category.Id });
    }
}
