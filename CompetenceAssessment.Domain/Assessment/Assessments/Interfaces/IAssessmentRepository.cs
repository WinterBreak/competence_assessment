namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentRepository
{
    Task<Assessment?> GetAssessmentAsync(AssessmentQuery query, CancellationToken token = default);
    
    Task<List<Assessment>> GetAssessmentsAsync(AssessmentQuery query, CancellationToken token = default);
    
    Task AddAssessmentAsync(Assessment assessment, CancellationToken token = default);
    
    Task UpdateAssessmentAsync(Assessment assessment, CancellationToken token = default);
    
    Task RemoveAssessmentAsync(int id, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}