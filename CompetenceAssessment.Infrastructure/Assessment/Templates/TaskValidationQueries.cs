using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskValidationQueries(AssessmentContext context): ITaskValidationQueries
{
    
    public async Task<bool> IsTaskExistAsync(string text, TaskType type, CancellationToken token = default)
        => await context.Tasks.AnyAsync(t => t.TaskTypeId == (int)type && t.Text == text, token);

    public async Task<bool> IsUsedInTemplateAsync(int taskId, CancellationToken token = default)
        => await context.TemplateDetails.AnyAsync(t => t.TaskId == taskId, token);
}