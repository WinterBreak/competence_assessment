namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentStateService
{
    Task ProceedStateAsync(Assessment assessment);
}