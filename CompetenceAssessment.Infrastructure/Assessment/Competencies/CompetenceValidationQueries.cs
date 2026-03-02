using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessment.Competencies;

public class CompetenceValidationQueries: ICompetenceValidationQueries
{
    private readonly AssessmentContext _context;

    public CompetenceValidationQueries(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsNameTakenAsync(string name, CancellationToken token = default)
        => await _context.Competences.AnyAsync(c => c.Name == name, token);

    public async Task<bool> IsUsedInModelsAsync(int competenceId, CancellationToken token = default)
        => await _context.CompetenceModelDetails
            .AsNoTracking()
            .AnyAsync(d => d.CompetenceId == competenceId, token);
}