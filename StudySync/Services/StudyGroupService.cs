using Microsoft.EntityFrameworkCore;
using StudySync.Data;
using StudySync.Models;

namespace StudySync.Services;

public class StudyGroupService
{
    private readonly ApplicationDbContext _context;

    public StudyGroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    private string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public async Task<StudyGroup> CreateGroupAsync(string name, string? description, string userId)
    {
        var group = new StudyGroup
        {
            Name = name,
            Description = description,
            JoinCode = GenerateJoinCode(),
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.StudyGroups.Add(group);
        await _context.SaveChangesAsync();

        var membership = new GroupMembership
        {
            UserId = userId,
            StudyGroupId = group.Id,
            JoinedAt = DateTime.UtcNow
        };
        _context.GroupMemberships.Add(membership);
        await _context.SaveChangesAsync();

        return group;
    }

    public async Task<(bool Success, string Message)> JoinGroupAsync(string joinCode, string userId)
    {
        var group = await _context.StudyGroups
            .FirstOrDefaultAsync(g => g.JoinCode == joinCode.ToUpper());

        if (group is null)
        {
            return (false, "No group found with that join code.");
        }

        var alreadyMember = await _context.GroupMemberships
            .AnyAsync(gm => gm.UserId == userId && gm.StudyGroupId == group.Id);

        if (alreadyMember)
        {
            return (false, "You are already a member of this group.");
        }

        var membership = new GroupMembership
        {
            UserId = userId,
            StudyGroupId = group.Id,
            JoinedAt = DateTime.UtcNow
        };
        _context.GroupMemberships.Add(membership);
        await _context.SaveChangesAsync();

        return (true, $"Successfully joined \"{group.Name}\"!");
    }

    public async Task<List<StudyGroup>> GetUserGroupsAsync(string userId)
    {
        return await _context.GroupMemberships
            .Where(gm => gm.UserId == userId)
            .Include(gm => gm.StudyGroup)
            .Select(gm => gm.StudyGroup)
            .ToListAsync();
    }

    public async Task<StudyGroup?> GetGroupByIdAsync(int groupId)
    {
        return await _context.StudyGroups
            .Include(g => g.Tasks)
            .FirstOrDefaultAsync(g => g.Id == groupId);
    }

    public async Task<bool> IsUserMemberAsync(int groupId, string userId)
    {
        return await _context.GroupMemberships
            .AnyAsync(gm => gm.StudyGroupId == groupId && gm.UserId == userId);
    }
}
