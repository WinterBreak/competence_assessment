using CompetenceAssessment.Core.Models;
using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;
using A = System.Threading.Tasks;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskRepository: ITaskRepository
{
        private readonly AssessmentContext _context;

    public TaskRepository(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<ITask?> GetTaskAsync(TaskQuery query
        , CancellationToken token = default)
    {
        var competencies = await GetTasksAsync(query, token);
        return competencies.SingleOrDefault();
    }

    public async Task<List<ITask>> GetTasksAsync(TaskQuery query
        , CancellationToken token = default)
    {
        var specification = new TaskSpecificationBuilder().WithQuery(query).Build();
        var tasks = GetAllTasks()
            .Where(specification)
            .OrderBy(t => t.Text)
            .AsEnumerable();
        
        return tasks
            .Select(t =>
            {
                var answers = t.Answers
                    .Select(a => new Domain.Assessment.Answer(a.Id, a.TaskId, a.Text, a.IsCorrect))
                    .ToList();
                return new ITask(t.Id, t.Text, (TaskType)t.TaskTypeId, answers);
            })
            .ToList();
    }
    
    public async Task<PaginatedResponse<ITask>> GetPaginatedTasksAsync(TaskQuery query
        , CancellationToken token = default)
    {
        var specification = new TaskSpecificationBuilder().WithQuery(query).Build();
        var tasks = GetAllTasks()
            .Where(specification)
            .OrderBy(t => t.Text)
            .Skip(query.PageSize * (query.Page - 1))
            .Take(query.PageSize)
            .AsEnumerable();
        
        var bllTasks = tasks
                .Select(t =>
                {
                    var answers = t.Answers
                        .Select(a => new Domain.Assessment.Answer(a.Id, a.TaskId, a.Text, a.IsCorrect))
                        .ToList();
                    return new ITask(t.Id, t.Text, (TaskType)t.TaskTypeId, answers);
                })
                .ToList();
        var totalCount = _context.Tasks.Where(specification).Count();
        return new PaginatedResponse<ITask>
        {
            Items = bllTasks,
            CurrentPage = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async A.Task AddTaskAsync(ITask task
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        var newTask = new Task((int)task.Type, task.Text);
        await _context.Tasks.AddAsync(newTask, token);

        if (task.Answers != null)
        {
            var newAnswers = task.Answers.Select(a 
                => new Answer(newTask, a.Text, a.IsCorrect)).ToList();
            await _context.Answers.AddRangeAsync(newAnswers, token);
        }
    }

    public async A.Task UpdateTaskAsync(ITask task
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        var updatingTask = await GetTaskByIdAsync(task.Id, token);
        
        updatingTask.Text = task.Text;
        updatingTask.TaskTypeId = (int)task.Type;

        if (task.Answers == null)
        {
            return;
        }
        await UpdateAnswers(task.Answers, updatingTask.Answers.ToList(), updatingTask.Id, token);
    }

    public async A.Task RemoveTaskAsync(int id, CancellationToken token = default)
    {
        var removingCategory = await GetTaskByIdAsync(id, token);
        _context.Tasks.Remove(removingCategory);
    }

    public async A.Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);

    private async A.Task AddAnswers(List<Domain.Assessment.Answer> newAnswers, int taskId
        , CancellationToken token = default)
    {
        if (newAnswers is null || !newAnswers.Any())
        {
            return;
        }
        
        var answers = newAnswers
            .Select(a => new Answer(taskId, a.Text, a.IsCorrect))
            .ToList();
        await _context.Answers.AddRangeAsync(answers, token);
    }
    
    private async A.Task UpdateAnswers(List<Domain.Assessment.Answer> updatedAnswers
        , List<Answer> details, int taskId, CancellationToken token = default)
    {
        if (updatedAnswers is null || !updatedAnswers.Any())
        {
            return;
        }

        if (details is null || !details.Any())
        {
            await AddAnswers(updatedAnswers, taskId, token);
            return;
        }
        
        var existingAnswersDict = details.ToDictionary(d => d.Text);
        var updatedAnswersDict = updatedAnswers.ToDictionary(w => w.Id);
        
        foreach (var existingDetail in details)
        {
            if (updatedAnswersDict.TryGetValue(existingDetail.Id, out var updatedWeight))
            {
                existingDetail.Text = updatedWeight.Text;
                existingDetail.IsCorrect = updatedWeight.IsCorrect;
            }
        }
        
        var answersToAdd = updatedAnswers
            .Where(w => !existingAnswersDict.ContainsKey(w.Text))
            .Select(w => new Answer(w.TaskId, w.Text, w.IsCorrect))
            .ToList();
        await _context.Answers.AddRangeAsync(answersToAdd, token);
        
        var answersToRemove = details
            .Where(d => !updatedAnswersDict.ContainsKey(d.TaskId))
            .ToList();
    
        _context.Answers.RemoveRange(answersToRemove);
    }
    
    private IQueryable<Task> GetAllTasks() => _context.Tasks.Include(t => t.Answers);

    private async Task<Task> GetTaskByIdAsync(int id, CancellationToken token = default)
    {
        var specification = new TaskSpecificationBuilder().WithIds(new List<int> { id }).Build();
        return await GetAllTasks()
            .Where(specification)
            .SingleAsync(token);
    }
}