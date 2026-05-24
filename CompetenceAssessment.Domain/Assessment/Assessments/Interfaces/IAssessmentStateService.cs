namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentStateService
{
    void ProceedState(Assessment assessment);
}