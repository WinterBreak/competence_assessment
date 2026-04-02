namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentValidationQueries
{
    Task<bool> IsAssessmentStartedAsync(int candidateId, int templateId, AssessmentType type
                                 , CancellationToken token = default);
    
    Task<bool> IsExistAsync(int id, CancellationToken token = default);
}