using Microsoft.EntityFrameworkCore;
using StudySync.Data;
using StudySync.Models;

namespace StudySync.Services;

/// <summary>A group member's id and display name, used for the "assign to" list.</summary>
public record MemberInfo(string UserId, string Name);

/// <summary>
/// Business logic for tasks (create, read, update, delete). Every method checks that the
/// calling user belongs to the group and returns a (Success, Message) pair so the UI can
/// show friendly feedback instead of crashing.
/// </summary>
public class TaskService
{
    private readonly ApplicationDbContext _context;

    public TaskService(ApplicationDbContext context)
    {
        _context = context;
    }

    private Task<bool> IsMemberAsync(int groupId, string userId) =>
        _context.GroupMemberships.AnyAsync(m => m.StudyGroupId == groupId && m.UserId == userId);

    /// <summary>Returns all tasks in a group, ordered by status then due date.</summary>
    public async Task<List<TaskItem>> GetTasksForGroupAsync(int groupId)
    {
        return await _context.TaskItems
            .AsNoTracking()
            .Where(t => t.StudyGroupId == groupId)
            .OrderBy(t => t.Status)
            .ThenBy(t => t.DueDate)
            .ToListAsync();
    }

    /// <summary>Returns the members of a group with display names.</summary>
    public async Task<List<MemberInfo>> GetMembersAsync(int groupId)
    {
        var ids = await _context.GroupMemberships
            .Where(m => m.StudyGroupId == groupId)
            .Select(m => m.UserId)
            .ToListAsync();

        return await _context.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new MemberInfo(u.Id, u.UserName ?? "Unknown"))
            .ToListAsync();
    }

    /// <summary>Creates a task. The caller must be a member of the group.</summary>
    public async Task<(bool Success, string Message)> CreateTaskAsync(TaskItem task, string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return (false, "A task needs a title.");
            if (!await IsMemberAsync(task.StudyGroupId, userId))
                return (false, "You must be a member of this group to add tasks.");
            if (task.AssignedToUserId is not null && !await IsMemberAsync(task.StudyGroupId, task.AssignedToUserId))
                return (false, "You can only assign tasks to members of this group.");

            task.Id = 0;
            task.CreatedByUserId = userId;
            task.CreatedAt = DateTime.UtcNow;
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();
            return (true, $"Task \"{task.Title}\" was added.");
        }
        catch (DbUpdateException)
        {
            return (false, "The task could not be saved. Please try again.");
        }
    }

    /// <summary>Updates title, description, due date, status and assignee of a task.</summary>
    public async Task<(bool Success, string Message)> UpdateTaskAsync(TaskItem updated, string userId)
    {
        try
        {
            var existing = await _context.TaskItems.FindAsync(updated.Id);
            if (existing is null)
                return (false, "That task no longer exists.");
            if (!await IsMemberAsync(existing.StudyGroupId, userId))
                return (false, "You must be a member of this group to edit tasks.");
            if (string.IsNullOrWhiteSpace(updated.Title))
                return (false, "A task needs a title.");
            if (updated.AssignedToUserId is not null && !await IsMemberAsync(existing.StudyGroupId, updated.AssignedToUserId))
                return (false, "You can only assign tasks to members of this group.");

            existing.Title = updated.Title;
            existing.Description = updated.Description;
            existing.DueDate = updated.DueDate;
            existing.Status = updated.Status;
            existing.AssignedToUserId = updated.AssignedToUserId;
            await _context.SaveChangesAsync();
            return (true, $"Task \"{existing.Title}\" was updated.");
        }
        catch (DbUpdateException)
        {
            return (false, "The task could not be updated. Please try again.");
        }
    }

    /// <summary>Quick status change (Not started / In progress / Completed).</summary>
    public async Task<(bool Success, string Message)> UpdateStatusAsync(int taskId, TaskItemStatus status, string userId)
    {
        try
        {
            var existing = await _context.TaskItems.FindAsync(taskId);
            if (existing is null)
                return (false, "That task no longer exists.");
            if (!await IsMemberAsync(existing.StudyGroupId, userId))
                return (false, "You must be a member of this group to change tasks.");

            existing.Status = status;
            await _context.SaveChangesAsync();
            return (true, $"\"{existing.Title}\" is now marked {status}.");
        }
        catch (DbUpdateException)
        {
            return (false, "The status could not be changed. Please try again.");
        }
    }

    /// <summary>Deletes a task. Only the task's creator or the group's creator may delete it.</summary>
    public async Task<(bool Success, string Message)> DeleteTaskAsync(int taskId, string userId)
    {
        try
        {
            var existing = await _context.TaskItems
                .Include(t => t.StudyGroup)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            if (existing is null)
                return (false, "That task no longer exists.");
            if (existing.CreatedByUserId != userId && existing.StudyGroup.CreatedByUserId != userId)
                return (false, "Only the task's creator or the group's creator can delete it.");

            _context.TaskItems.Remove(existing);
            await _context.SaveChangesAsync();
            return (true, "Task deleted.");
        }
        catch (DbUpdateException)
        {
            return (false, "The task could not be deleted. Please try again.");
        }
    }
}
