namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceRepository
{
    Task<Competence?> GetCompetenceAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    Task<List<Competence>> GetCompetencesAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    Task AddCompetenceAsync(CreateCompetenceCommand command, CancellationToken cancellationToken = default);
    
    Task UpdateCompetenceAsync(UpdateCompetenceCommand command, CancellationToken cancellationToken = default);
    
    Task DeleteCompetenceAsync(DeleteCompetenceCommand command, CancellationToken cancellationToken = default);
    
    Task SaveAllChangesAsync(CancellationToken cancellationToken = default);
}