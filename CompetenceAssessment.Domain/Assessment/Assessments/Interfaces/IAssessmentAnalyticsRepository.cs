namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentAnalyticsRepository
{
    Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(CancellationToken token = default);
    
    Task<CompetenceDevelopmentData> GetCompetenceDevelopmentAsync(int employeeId
        ,  CancellationToken token = default);
}