using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateValidationQueries: ITemplateValidationQueries
{
    private readonly AssessmentContext _context;

    public TemplateValidationQueries(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsNameTakenAsync(string name, CancellationToken token = default)
        => await _context.Templates.AnyAsync(t => t.Name == name, token);

    public async Task<bool> IsUsedInAssessmentAsync(int templateId, CancellationToken token = default)
        => await _context.Assessments.AnyAsync(a => a.TemplateId == templateId, token);
}