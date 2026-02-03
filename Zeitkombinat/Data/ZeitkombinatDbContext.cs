using Microsoft.EntityFrameworkCore;
using Zeitkombinat.Models;

namespace Zeitkombinat.Data;

public class ZeitkombinatDbContext : DbContext {
    public DbSet<Project> Projects { get; set; }
    public DbSet<Story> Stories { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<WorkSession> WorkSessions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceTask> InvoiceTasks { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Data Source=hourglass.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var projectBuilder = modelBuilder.Entity<Project>();
        projectBuilder
            .HasMany(p => p.Stories)
            .WithOne(s => s.Project)
            .HasForeignKey(s => s.ProjectId);
        projectBuilder
            .HasMany(p => p.Invoices)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId);

        modelBuilder.Entity<Story>()
            .HasMany(s => s.Tasks)
            .WithOne(t => t.Story)
            .HasForeignKey(t => t.StoryId);

        modelBuilder.Entity<TaskItem>()
            .HasMany(t => t.WorkSessions)
            .WithOne(w => w.TaskItem)
            .HasForeignKey(w => w.TaskItemId);

        modelBuilder.Entity<Invoice>()
            .HasMany(i => i.InvoiceTasks)
            .WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId);

        modelBuilder.Entity<InvoiceTask>()
            .HasOne(it => it.TaskItem)
            .WithMany()
            .HasForeignKey(it => it.TaskItemId);
    }
}
