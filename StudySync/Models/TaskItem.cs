using System.ComponentModel.DataAnnotations;

namespace StudySync.Models;

public enum TaskItemStatus
{
    NotStarted,
    InProgress,
    Completed
}

public class TaskItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public TaskItemStatus Status { get; set; } = TaskItemStatus.NotStarted;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int StudyGroupId { get; set; }
    public string? AssignedToUserId { get; set; }
    public string CreatedByUserId { get; set; } = string.Empty;

    public StudyGroup StudyGroup { get; set; } = null!;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
