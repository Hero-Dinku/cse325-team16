using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudySync.Models;

namespace StudySync.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<StudyGroup> StudyGroups { get; set; }
    public DbSet<GroupMembership> GroupMemberships { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Prevent a user from joining the same group twice
        builder.Entity<GroupMembership>()
            .HasIndex(gm => new { gm.UserId, gm.StudyGroupId })
            .IsUnique();

        // Cascade delete: deleting a group removes its tasks
        builder.Entity<TaskItem>()
            .HasOne(t => t.StudyGroup)
            .WithMany(g => g.Tasks)
            .HasForeignKey(t => t.StudyGroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade delete: deleting a task removes its comments
        builder.Entity<Comment>()
            .HasOne(c => c.TaskItem)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
