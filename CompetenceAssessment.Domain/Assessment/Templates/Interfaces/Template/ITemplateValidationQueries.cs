namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, CancellationToken token = default);
    
    Task<bool> IsUsedInAssessmentAsync(int templateId, CancellationToken token = default);
}