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
        return await GetAllTasks()
            .Where(specification)
            .Select(t => new ITask(t.Id, t.Text, t.Answer, (TaskType)t.TaskTypeId))
            .ToListAsync(token);
    }

    public async A.Task AddTaskAsync(ITask task
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        var newTask = new Task((int)task.Type, task.Text, task.Answer);
        await _context.Tasks.AddAsync(newTask, token);
    }

    public async A.Task UpdateTaskAsync(ITask task
        , CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(task);
        
        var updatingTask = await GetTaskByIdAsync(task.Id, token);
        
        updatingTask.Text = task.Text;
        updatingTask.Answer = task.Answer;
        updatingTask.TaskTypeId = (int)task.Type;
    }

    public async A.Task RemoveTaskAsync(int id, CancellationToken token = default)
    {
        var removingCategory = await GetTaskByIdAsync(id, token);
        _context.Tasks.Remove(removingCategory);
    }

    public async A.Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);
    
    private IQueryable<Task> GetAllTasks() => _context.Tasks;

    private async Task<Task> GetTaskByIdAsync(int id, CancellationToken token = default)
    {
        var specification = new TaskSpecificationBuilder().WithIds(new List<int> { id }).Build();
        return await GetAllTasks()
            .Where(specification)
            .SingleAsync(token);
    }
}