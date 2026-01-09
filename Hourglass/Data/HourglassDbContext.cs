using Microsoft.EntityFrameworkCore;
using Hourglass.Models;

namespace Hourglass.Data;

public class HourglassDbContext : DbContext {
    public DbSet<Project> Projects { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<WorkSession> WorkSessions { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=hourglass.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Stories)
            .WithOne(s => s.Project)
            .HasForeignKey(s => s.ProjectId);

        modelBuilder.Entity<Story>()
            .HasMany(s => s.Tasks)
            .WithOne(t => t.Story)
            .HasForeignKey(t => t.StoryId);

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.WorkSessions)
            .WithOne(w => w.TaskItem)
            .HasForeignKey(w => w.TaskItemId);
    }
}
