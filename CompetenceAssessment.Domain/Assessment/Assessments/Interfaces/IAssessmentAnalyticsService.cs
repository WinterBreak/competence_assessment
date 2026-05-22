namespace CompetenceAssessment.Domain.Assessment;

public interface IAssessmentAnalyticsService
{
    Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(CancellationToken token = default);
    
    Task<List<PositionCompetencies>> GetPositionsCompetenciesAsync(CancellationToken token = default);

    Task<CompetenceDevelopmentData> GetCompetenceDevelopmentAsync(int employeeId
        , CancellationToken token = default);

    Task<List<DepartmentCompetencies>> GetDepartmentCompetenciesAsync(CancellationToken token = default);
}