namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceModelRepository
{
    Task<CompetenceModel?> GetCompetenceModelAsync(CompetenceModelQuery query
        , CancellationToken token = default);
    
    Task<List<CompetenceModel>> GetCompetenceModelsAsync(CompetenceModelQuery query
        , CancellationToken token = default);
    
    Task AddCompetenceModelAsync(CompetenceModel competenceModel, CancellationToken token = default);
    
    Task UpdateCompetenceModelAsync(CompetenceModel competenceModel, CancellationToken token = default);
    
    Task RemoveCompetenceModelAsync(int id, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}