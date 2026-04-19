namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentCalcStrategy
{
    AssessmentCalculation Calculate(Assessment assessment);
}