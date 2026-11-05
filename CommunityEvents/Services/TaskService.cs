using Microsoft.EntityFrameworkCore;
using CommunityEvents.Data;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class TaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db) => _db = db;

    public async Task<AppTask?> GetTaskAsync(int id) =>
        await _db.Tasks.Include(t => t.AssignedMember).Include(t => t.Issue).FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<AppTask>> GetEventTasksAsync(int eventId) =>
        await _db.Tasks.Include(t => t.Issue).Include(t => t.AssignedMember)
            .Where(t => t.Issue.EventId == eventId).ToListAsync();

    public async Task CreateTaskAsync(AppTask task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateTaskAsync(AppTask task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(int taskId, AppTaskStatus status)
    {
        var task = await GetTaskAsync(taskId);
        if (task != null)
        {
            task.Status = status;
            await UpdateTaskAsync(task);
        }
    }

    public async Task AssignTaskAsync(int taskId, int? memberId)
    {
        var task = await GetTaskAsync(taskId);
        if (task != null)
        {
            task.AssignedMemberId = memberId;
            await UpdateTaskAsync(task);
        }
    }

    public async Task<decimal> GetMemberWorkloadAsync(int memberId, List<AppTask> allTasks)
    {
        var assignedTasks = allTasks.Where(t => t.AssignedMemberId == memberId &&
            t.Status != AppTaskStatus.Completed).ToList();

        var effort = assignedTasks.Sum(t => t.EffortPoints);
        var money = assignedTasks.Sum(t => t.MoneyValue);

        return effort + money;
    }
}
