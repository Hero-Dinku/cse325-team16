namespace StudySync.Models;

public class GroupMembership
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    public int StudyGroupId { get; set; }

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public StudyGroup StudyGroup { get; set; } = null!;
}
