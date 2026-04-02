namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, int? id = null, CancellationToken token = default);
    
    Task<bool> IsUsedInModelsAsync(int competenceId, CancellationToken token = default);
    
    Task<bool> IsCompetenceExistsAsync(int competenceId, CancellationToken token = default);
}