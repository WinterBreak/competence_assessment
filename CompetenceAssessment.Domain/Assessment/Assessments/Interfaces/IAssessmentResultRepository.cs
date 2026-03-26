namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentResultRepository
{
    Task<List<AssessmentResult>> GetAssessmentsAsync(AssessmentResultQuery query, CancellationToken token = default);
    
    Task AddAssessmentsAsync(IEnumerable<AssessmentResult> results, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}