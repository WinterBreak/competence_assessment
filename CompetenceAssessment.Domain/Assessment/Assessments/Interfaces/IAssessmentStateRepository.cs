namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentStateRepository
{
    AssessmentState UpdateStateAsync(int assessmentId, CancellationToken token = default);

    Task SaveAllChanges();
}