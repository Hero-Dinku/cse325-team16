using System.ComponentModel.DataAnnotations;

namespace StudySync.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    [StringLength(1000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TaskItemId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public TaskItem TaskItem { get; set; } = null!;
}
