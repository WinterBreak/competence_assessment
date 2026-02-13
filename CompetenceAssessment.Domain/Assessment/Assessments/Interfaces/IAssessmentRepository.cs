namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentRepository
{
    Task<Assessment?> GetAssessmentAsync(AssessmentQuery query, CancellationToken cancellationToken = default);
    
    Task<List<Assessment>> GetAssessmentsAsync(AssessmentQuery query, CancellationToken cancellationToken = default);
    
    Task AddAssessmentAsync(Assessment assessment, CancellationToken cancellationToken = default);
    
    Task UpdateAssessmentAsync(Assessment assessment, CancellationToken cancellationToken = default);
    
    Task RemoveAssessmentAsync(Assessment assessment, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}