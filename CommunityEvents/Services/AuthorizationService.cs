using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class AuthorizationService
{
    private readonly AppDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthorizationService(AppDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<Member?> GetMemberAsync(int eventId, string userId)
    {
        return await _db.Members
            .FirstOrDefaultAsync(m => m.EventId == eventId && m.UserId == userId);
    }

    public async Task<bool> IsOrganizerAsync(int eventId, string userId)
    {
        var member = await GetMemberAsync(eventId, userId);
        return member?.Role == MemberRole.Organizer;
    }

    public async Task<bool> CanEditEventAsync(int eventId, string userId)
    {
        return await IsOrganizerAsync(eventId, userId);
    }

    public async Task<bool> CanEditTaskAsync(int taskId, string userId)
    {
        var task = await _db.Tasks
            .Include(t => t.Issue)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task?.Issue == null) return false;

        var isOrganizer = await IsOrganizerAsync(task.Issue.EventId, userId);
        if (isOrganizer) return true;

        var isAssigned = task.AssignedMemberId != null &&
            (await GetMemberAsync(task.Issue.EventId, userId))?.Id == task.AssignedMemberId;
        return isAssigned;
    }

    public async Task<bool> CanCommentAsync(int taskId, int eventId, string userId)
    {
        var member = await GetMemberAsync(eventId, userId);
        return member != null;
    }

    public async Task<bool> CanDeleteCommentAsync(int commentId, string userId)
    {
        var comment = await _db.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment?.Author == null) return false;
        return comment.Author.UserId == userId;
    }
}
