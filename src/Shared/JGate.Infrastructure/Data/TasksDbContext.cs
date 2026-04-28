using JGate.Domain.Entities.Tasks;
using Microsoft.EntityFrameworkCore;

namespace JGate.Infrastructure.Data;

public class TasksDbContext(DbContextOptions<TasksDbContext> options) : DbContext(options)
{
    public DbSet<WorkTask> Tasks => Set<WorkTask>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();
    public DbSet<TaskComment> TaskComments => Set<TaskComment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkTask>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).IsRequired().HasMaxLength(300);
            e.HasOne(x => x.Category).WithMany(x => x.Tasks).HasForeignKey(x => x.CategoryId);
        });

        modelBuilder.Entity<TaskAssignment>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Task).WithMany(x => x.Assignments).HasForeignKey(x => x.TaskId);
        });

        modelBuilder.Entity<TaskComment>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Content).IsRequired().HasMaxLength(2000);
            e.HasOne(x => x.Task).WithMany(x => x.Comments).HasForeignKey(x => x.TaskId);
        });
    }
}
