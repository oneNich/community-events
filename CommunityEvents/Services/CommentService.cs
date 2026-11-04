using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class CommentService
{
    private readonly AppDbContext _db;

    public CommentService(AppDbContext db) => _db = db;

    public async Task<List<Comment>> GetTaskCommentsAsync(int taskId) =>
        await _db.Comments.Where(c => c.TaskId == taskId)
            .Include(c => c.Author).OrderBy(c => c.CreatedAt).ToListAsync();

    public async Task AddCommentAsync(Comment comment)
    {
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteCommentAsync(int id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment != null)
        {
            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();
        }
    }
}

public class ActivityLogService
{
    private readonly AppDbContext _db;

    public ActivityLogService(AppDbContext db) => _db = db;

    public async Task<List<ActivityLog>> GetEventActivityAsync(int eventId) =>
        await _db.ActivityLogs.Where(al => al.EventId == eventId)
            .Include(al => al.RelatedMember).OrderByDescending(al => al.CreatedAt).ToListAsync();

    public async Task LogAsync(int eventId, int? memberId, string action, string details)
    {
        var log = new ActivityLog
        {
            EventId = eventId,
            RelatedMemberId = memberId,
            Action = action,
            Details = details,
            CreatedAt = DateTime.UtcNow
        };
        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}
