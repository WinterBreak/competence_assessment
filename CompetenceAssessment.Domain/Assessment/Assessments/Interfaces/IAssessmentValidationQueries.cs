namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentValidationQueries
{
    Task<bool> IsAssessmentStarted(int candidateId, int templateId, AssessmentType type
                                 , CancellationToken token = default);
}