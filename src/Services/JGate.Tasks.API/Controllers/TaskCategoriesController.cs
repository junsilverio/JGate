using JGate.Domain.Entities.Tasks;
using JGate.Infrastructure.Data;
using JGate.Tasks.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JGate.Tasks.API.Controllers;

[ApiController]
[Route("api/task-categories")]
[Authorize]
public class TaskCategoriesController(TasksDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskCategoryDto>>> GetAll()
    {
        var items = await db.TaskCategories.Where(c => !c.IsDeleted)
            .Select(c => new TaskCategoryDto(c.Id, c.Name, c.Description, c.Color))
            .ToListAsync();
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskCategoryRequest request)
    {
        var category = new TaskCategory { Name = request.Name, Description = request.Description, Color = request.Color };
        db.TaskCategories.Add(category);
        await db.SaveChangesAsync();
        return Ok(new { id = category.Id });
    }
}
