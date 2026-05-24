using CompetenceAssessment.Infrastructure.Assessment;
using CompetenceAssessment.Infrastructure.Assessments;
using Microsoft.EntityFrameworkCore;
using BLL = CompetenceAssessment.Domain.Assessment;
using Task = System.Threading.Tasks.Task;

namespace CompetenceAssessment.Infrastructure;

public class AssessmentResultRepository: BLL.IAssessmentResultRepository
{
    private readonly AssessmentContext _context;
    private readonly BLL.ITaskRepository _taskRepository;

    public AssessmentResultRepository(AssessmentContext context
        , BLL.ITaskRepository taskRepository)
    {
        _context = context;
        _taskRepository = taskRepository;
    }
    
    public async Task<List<BLL.AssessmentResult>> GetAssessmentsAsync(BLL.AssessmentResultQuery query
                                                                    , CancellationToken token = default)
    {
        var specification = new AssessmentResultSpecificationBuilder().WithAssessmentIds(query.AssessmentIds).Build();
        var results = await _context.AssessmentResults.Where(specification).ToListAsync(token);
        
        var taskIds = results.Select(r => r.TaskId).ToList();
        var taskQuery = new BLL.TaskQuery(ids: taskIds);
        var tasks = await _taskRepository.GetTasksAsync(taskQuery, token);
        
        return results
            .Select(ar =>
            {
                var task = tasks.Single(t => t.Id == ar.TaskId);
                return new BLL.AssessmentResult(ar.Id, ar.AssessmentId, task, ar.Comment, ar.Answer, ar.Score);
            })
            .ToList();
    }

    public async Task AddAssessmentsAsync(IEnumerable<BLL.AssessmentResult> results
                                        , CancellationToken token = default)
    {
        if (results is null || !results.Any())
        {
            return;
        }

        var taskIds = results.Select(r => r.Task.Id).ToList();
        var taskAnswers = _context.Answers
            .Where(a => taskIds.Contains(a.TaskId) && a.IsCorrect)
            .ToDictionary(a => a.TaskId, a => a.Text);
        var scale = _context.Assessments
            .Single(a => a.Id == results.First().AssessmentId).Template.ScaleId;
        var newResults = new List<AssessmentResult>();
        foreach (var result in results)
        {
            taskAnswers.TryGetValue(result.Task.Id, out var answer);
            var score = answer == result.Answer ? scale : 0;
            var newResult = new AssessmentResult(result.Comment, result.Answer, score
                                               , result.AssessmentId, result.Task.Id);
            newResults.Add(newResult);
        }
        
        await _context.AssessmentResults.AddRangeAsync(newResults, token);
    }

    public async Task SaveAllChangesAsync(CancellationToken token = default)
        => await _context.SaveChangesAsync(token);
}