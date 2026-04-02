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
    
    public async Task<bool> IsNameTakenAsync(string name, int? id = null, CancellationToken token = default)
        => await _context.CompetenceModels.AnyAsync(cm => ((id == null || id == default(int))
                                                       || (id != null && id != default(int) && cm.Id != id))
                                                       && cm.Name == name, token);

    public async Task<bool> IsUsedInTemplateAsync(int id, CancellationToken token = default)
        => await _context.Templates.AnyAsync(t => t.CompetenceModelId == id, token);

    public async Task<bool> IsModelExist(int id, CancellationToken token = default)
        => await _context.CompetenceModels.AnyAsync(cm => cm.Id == id, token);
}