namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentAnalyticsService
{
    Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(CancellationToken token = default);
}