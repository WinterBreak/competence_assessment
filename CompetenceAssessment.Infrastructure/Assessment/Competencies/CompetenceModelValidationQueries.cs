using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessment.Competencies;

public class CompetenceModelValidationQueries: ICompetenceModelValidationQueries
{
    private readonly AssessmentContext _context;

    public CompetenceModelValidationQueries(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsNameTakenAsync(string name, CancellationToken token = default)
        => await _context.CompetenceModels.AnyAsync(cm => cm.Name == name, token);

    public async Task<bool> IsUsedInTemplateAsync(int modelId, CancellationToken token = default)
        => await _context.Templates.AnyAsync(t => t.CompetenceModelId == modelId, token);
}