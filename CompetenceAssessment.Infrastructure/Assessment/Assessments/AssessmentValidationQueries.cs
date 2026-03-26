using CompetenceAssessment.Domain.Assessment;
using Microsoft.EntityFrameworkCore;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentValidationQueries: IAssessmentValidationQueries
{
    private readonly AssessmentContext _context;

    public AssessmentValidationQueries(AssessmentContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsAssessmentStarted(int candidateId, int templateId, AssessmentType type
                                              , CancellationToken token = default)
        => await _context.Assessments.AnyAsync(a => a.UserId == candidateId 
                                                           && a.AssessmentTypeId == (int)type
                                                           && a.IsFinished == false
                                                           && a.TemplateId == templateId, token);
}