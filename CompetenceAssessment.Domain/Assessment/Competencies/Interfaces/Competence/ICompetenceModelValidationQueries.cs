namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelValidationQueries
{
    Task<bool> IsNameTakenAsync(string name, CancellationToken token = default);
    
    Task<bool> IsUsedInTemplateAsync(int modelId, CancellationToken token = default);
}