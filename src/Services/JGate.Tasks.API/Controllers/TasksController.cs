using JGate.Domain.Entities.Tasks;
using JGate.Infrastructure.Data;
using JGate.Tasks.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskStatus = JGate.Domain.Enums.TaskStatus;
using TaskPriority = JGate.Domain.Enums.TaskPriority;

namespace JGate.Tasks.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController(TasksDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkTaskDto>>> GetAll([FromQuery] string? status = null, [FromQuery] string? priority = null)
    {
        var query = db.Tasks
            .Include(t => t.Category)
            .Include(t => t.Comments)
            .Where(t => !t.IsDeleted);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatus>(status, out var s))
            query = query.Where(t => t.Status == s);
        if (!string.IsNullOrEmpty(priority) && Enum.TryParse<TaskPriority>(priority, out var p))
            query = query.Where(t => t.Priority == p);

        var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        return Ok(tasks.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<WorkTaskDto>> GetById(Guid id)
    {
        var task = await db.Tasks.Include(t => t.Category).Include(t => t.Comments)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        if (task == null) return NotFound();
        return Ok(ToDto(task));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        if (!Enum.TryParse<TaskPriority>(request.Priority, out var priority))
            return BadRequest(new { message = "Invalid priority." });

        var task = new WorkTask
        {
            Title = request.Title, Description = request.Description,
            CategoryId = request.CategoryId, Priority = priority,
            DueDate = request.DueDate, AssignedToUserId = request.AssignedToUserId,
            AssignedToUserName = request.AssignedToUserName, Notes = request.Notes
        };
        db.Tasks.Add(task);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, new { id = task.Id });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task == null || task.IsDeleted) return NotFound();
        if (!Enum.TryParse<TaskStatus>(request.Status, out var newStatus))
            return BadRequest(new { message = "Invalid status." });
        task.Status = newStatus;
        task.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] CreateCommentRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task == null || task.IsDeleted) return NotFound();
        var comment = new TaskComment { TaskId = id, UserId = request.UserId, UserName = request.UserName, Content = request.Content };
        db.TaskComments.Add(comment);
        await db.SaveChangesAsync();
        return Ok(new { id = comment.Id });
    }

    [HttpPost("{id:guid}/assignments")]
    public async Task<IActionResult> Assign(Guid id, [FromBody] CreateAssignmentRequest request)
    {
        var task = await db.Tasks.FindAsync(id);
        if (task == null || task.IsDeleted) return NotFound();
        var assignment = new TaskAssignment { TaskId = id, UserId = request.UserId, UserName = request.UserName };
        db.TaskAssignments.Add(assignment);
        await db.SaveChangesAsync();
        return Ok(new { id = assignment.Id });
    }

    private static WorkTaskDto ToDto(WorkTask t) => new(
        t.Id, t.Title, t.Description, t.CategoryId, t.Category?.Name ?? string.Empty,
        t.Status.ToString(), t.Priority.ToString(), t.DueDate, t.AssignedToUserId, t.AssignedToUserName, t.Notes,
        t.Comments.Select(c => new TaskCommentDto(c.Id, c.UserId, c.UserName, c.Content, c.PostedAt)));
}
