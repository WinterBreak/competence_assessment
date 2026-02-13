namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelRepository
{
    Task<CompetenceModel?> GetCompetenceModelAsync(CompetenceModelQuery query
        , CancellationToken cancellationToken = default);
    
    Task<List<CompetenceModel>> GetCompetenceModelsAsync(CompetenceModelQuery query
        , CancellationToken cancellationToken = default);
    
    Task AddCompetenceModelAsync(CompetenceModel competenceModel, CancellationToken cancellationToken = default);
    
    Task UpdateCompetenceModelAsync(CompetenceModel competenceModel, CancellationToken cancellationToken = default);
    
    Task RemoveCompetenceModelAsync(CompetenceModel competenceModel, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}