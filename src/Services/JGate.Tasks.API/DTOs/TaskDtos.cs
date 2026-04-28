namespace JGate.Tasks.API.DTOs;

public record TaskCategoryDto(Guid Id, string Name, string? Description, string? Color);
public record CreateTaskCategoryRequest(string Name, string? Description, string? Color);

public record TaskCommentDto(Guid Id, string UserId, string UserName, string Content, DateTime PostedAt);
public record CreateCommentRequest(string UserId, string UserName, string Content);

public record TaskAssignmentDto(Guid Id, string UserId, string UserName, DateTime AssignedAt, bool IsActive);
public record CreateAssignmentRequest(string UserId, string UserName);

public record WorkTaskDto(Guid Id, string Title, string? Description, Guid CategoryId, string CategoryName, string Status, string Priority, DateTime? DueDate, string? AssignedToUserId, string? AssignedToUserName, string? Notes, IEnumerable<TaskCommentDto> Comments);
public record CreateTaskRequest(string Title, string? Description, Guid CategoryId, string Priority, DateTime? DueDate, string? AssignedToUserId, string? AssignedToUserName, string? Notes);
public record UpdateTaskStatusRequest(string Status);
