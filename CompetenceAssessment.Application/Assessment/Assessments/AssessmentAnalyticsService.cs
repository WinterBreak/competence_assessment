using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Application.Assessment;

public class AssessmentAnalyticsService: IAssessmentAnalyticsService
{
    private readonly IAssessmentAnalyticsRepository _repository;

    public AssessmentAnalyticsService(IAssessmentAnalyticsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<List<AssessmentParticipantCompetencies>> GetParticipantsCompetenciesAsync(
        CancellationToken token = default)
    {
        var competencies = await _repository.GetParticipantsCompetenciesAsync(token);
        foreach (var competence in competencies)
        {
            competence.ParticipantCompetences.ForEach(c => c.Level = DetermineLevel(c.Percentage));
        }
        
        return competencies;
    }
    
    private string DetermineLevel(decimal percentage)
    {
        if (percentage >= 90) return "Высокий";
        if (percentage >= 70) return "Выше среднего";
        if (percentage >= 50) return "Средний";
        if (percentage >= 30) return "Ниже среднего";
        return "Низкий";
    }
}