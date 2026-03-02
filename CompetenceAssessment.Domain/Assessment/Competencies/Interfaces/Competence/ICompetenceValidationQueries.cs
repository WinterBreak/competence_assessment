namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, CancellationToken token = default);
    
    Task<bool> IsUsedInModelsAsync(int competenceId, CancellationToken token = default);
}