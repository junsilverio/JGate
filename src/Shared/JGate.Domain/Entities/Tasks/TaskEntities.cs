using JGate.Domain.Common;
using JGate.Domain.Enums;
using TaskStatus = JGate.Domain.Enums.TaskStatus;

namespace JGate.Domain.Entities.Tasks;

public class TaskCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public ICollection<WorkTask> Tasks { get; set; } = new List<WorkTask>();
}

public class WorkTask : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public TaskCategory? Category { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.Open;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public DateTime? DueDate { get; set; }
    public string? AssignedToUserId { get; set; }
    public string? AssignedToUserName { get; set; }
    public string? Notes { get; set; }
    public ICollection<TaskComment> Comments { get; set; } = new List<TaskComment>();
    public ICollection<TaskAssignment> Assignments { get; set; } = new List<TaskAssignment>();
}

public class TaskAssignment : BaseEntity
{
    public Guid TaskId { get; set; }
    public WorkTask? Task { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}

public class TaskComment : BaseEntity
{
    public Guid TaskId { get; set; }
    public WorkTask? Task { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; } = DateTime.UtcNow;
}
