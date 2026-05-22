namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentCalcService
{
    Task<AssessmentCalculation> CalculateAsync(int assessmentId, CancellationToken token = default);
}