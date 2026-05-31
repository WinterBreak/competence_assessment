using CompetenceAssessment.Core.Models;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceRepository
{
    Task<Competence?> GetCompetenceAsync(CompetenceQuery query, CancellationToken token = default);
    
    Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query, CancellationToken token = default);

    Task<PaginatedResponse<Competence>> GetPaginatedCompetenciesAsync(CompetenceQuery query
        , CancellationToken token = default);
    
    Task AddCompetenceAsync(Competence competence, CancellationToken token = default);
    
    Task UpdateCompetenceAsync(Competence competence, CancellationToken token = default);
    
    // RemoveCompetenciesAsync - если на UI добавить чекбоксы для выбора
    
    Task RemoveCompetenceAsync(int id, CancellationToken token = default);
    
    Task SaveAllChangesAsync(CancellationToken token = default);
}