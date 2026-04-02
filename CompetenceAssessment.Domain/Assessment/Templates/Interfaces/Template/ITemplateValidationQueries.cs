namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, int? id = null, CancellationToken token = default);
    
    Task<bool> IsUsedInAssessmentAsync(int templateId, CancellationToken token = default);
    
    Task<bool> IsExistAsync(int id, CancellationToken token = default);
}