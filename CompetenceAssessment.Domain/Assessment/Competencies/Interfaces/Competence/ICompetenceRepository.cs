namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceRepository
{
    Task<Competence?> GetCompetenceAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    Task<List<Competence>> GetCompetenciesAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    // AddCompetenciesAsync - для импорта из файла
    
    Task AddCompetenceAsync(Competence competence, CancellationToken cancellationToken = default);
    
    Task UpdateCompetenceAsync(Competence competence, CancellationToken cancellationToken = default);
    
    // RemoveCompetenciesAsync - если на UI добавить чекбоксы для выбора
    
    Task RemoveCompetenceAsync(int id, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}