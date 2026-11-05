using Microsoft.EntityFrameworkCore;
using CommunityEvents.Models;

namespace CommunityEvents.Services;

public class TaskDistributionService
{
    private readonly TaskService _taskService;
    private readonly AppDbContext _db;

    public TaskDistributionService(TaskService taskService, AppDbContext db)
    {
        _taskService = taskService;
        _db = db;
    }

    public async Task<Dictionary<int, int?>> GeneratePreviewAsync(int eventId, decimal effortWeight, decimal moneyWeight)
    {
        var tasks = await _taskService.GetEventTasksAsync(eventId);
        var members = await _db.Members.Where(m => m.EventId == eventId).ToListAsync();
        var unassignedTasks = tasks
            .Where(t => t.AssignedMemberId == null && t.Status != AppTaskStatus.WorkingOn && t.Status != AppTaskStatus.Completed)
            .ToList();

        var maxEffort = tasks.Any() ? tasks.Max(t => t.EffortPoints) : 1;
        var maxMoney = tasks.Any() ? tasks.Max(t => t.MoneyValue) : 1m;

        foreach (var task in unassignedTasks)
        {
            var normEffort = maxEffort > 0 ? (decimal)task.EffortPoints / maxEffort : 0;
            var normMoney = maxMoney > 0 ? task.MoneyValue / maxMoney : 0m;
            task.Score = (effortWeight * normEffort) + (moneyWeight * normMoney);
        }

        var sortedTasks = unassignedTasks.OrderByDescending(t => t.Score).ThenBy(t => t.Id).ToList();

        var memberWorkloads = new Dictionary<int, decimal>();
        foreach (var member in members)
        {
            var workload = await _taskService.GetMemberWorkloadAsync(member.Id, tasks);
            memberWorkloads[member.Id] = workload;
        }

        var assignments = new Dictionary<int, int?>();
        foreach (var task in sortedTasks)
        {
            var bestMember = members.OrderBy(m => memberWorkloads[m.Id]).First();
            assignments[task.Id] = bestMember.Id;
            memberWorkloads[bestMember.Id] += task.EffortPoints + (double)task.MoneyValue;
        }

        return assignments;
    }

    public async Task ApplyAssignmentsAsync(int eventId, Dictionary<int, int?> assignments)
    {
        var tasks = await _db.Tasks.Include(t => t.Issue).Where(t => t.Issue.EventId == eventId).ToListAsync();
        foreach (var (taskId, memberId) in assignments)
        {
            var task = tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null && task.Status != AppTaskStatus.WorkingOn && task.Status != AppTaskStatus.Completed)
            {
                task.AssignedMemberId = memberId;
            }
        }
        await _db.SaveChangesAsync();
    }

    public async Task<Dictionary<int, WorkloadSummary>> GetFairnessAsync(int eventId)
    {
        var members = await _db.Members.Where(m => m.EventId == eventId).ToListAsync();
        var tasks = await _taskService.GetEventTasksAsync(eventId);
        var totalEffort = tasks.Sum(t => t.EffortPoints);
        var totalMoney = tasks.Sum(t => t.MoneyValue);

        var summary = new Dictionary<int, WorkloadSummary>();
        foreach (var member in members)
        {
            var memberTasks = tasks.Where(t => t.AssignedMemberId == member.Id && t.Status != AppTaskStatus.Completed).ToList();
            summary[member.Id] = new WorkloadSummary
            {
                MemberId = member.Id,
                MemberName = member.Name,
                TaskCount = memberTasks.Count(),
                TotalEffort = memberTasks.Sum(t => t.EffortPoints),
                TotalMoney = memberTasks.Sum(t => t.MoneyValue),
                EffortPercentage = totalEffort > 0 ? (memberTasks.Sum(t => t.EffortPoints) * 100m) / totalEffort : 0,
                MoneyPercentage = totalMoney > 0 ? (memberTasks.Sum(t => t.MoneyValue) * 100m) / totalMoney : 0
            };
        }
        return summary;
    }
}

public class WorkloadSummary
{
    public int MemberId { get; set; }
    public string MemberName { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int TotalEffort { get; set; }
    public decimal TotalMoney { get; set; }
    public decimal EffortPercentage { get; set; }
    public decimal MoneyPercentage { get; set; }
}
