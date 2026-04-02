using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskValidationQueries(AssessmentContext context): ITaskValidationQueries
{
    public async Task<bool> IsTaskExistAsync(string text, TaskType type, int? id = null
        , CancellationToken token = default)
    {
        return await context.Tasks.AnyAsync(t => ((id == null || id == default(int)) 
                                                   || (id != null && id != default(int) && t.Id != id))
                                                   && t.TaskTypeId == (int)type 
                                                   && t.Text == text, token);
    }

    public async Task<bool> IsUsedInTemplateAsync(int id, CancellationToken token = default)
        => await context.TemplateDetails.AnyAsync(t => t.TaskId == id, token);

    public async Task<bool> IsTaskExistAsync(int id, CancellationToken token = default)
        => await context.Tasks.AnyAsync(t => t.Id == id, token);
}