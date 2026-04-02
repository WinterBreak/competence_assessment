namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, int? id = null, CancellationToken token = default);
    
    Task<bool> IsUsedInTemplateAsync(int id, CancellationToken token = default);
    
    Task<bool> IsModelExist(int id, CancellationToken token = default);
}