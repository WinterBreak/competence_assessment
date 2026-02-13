using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ICompetenceService
{
    Task<Competence?> GetCompetenceAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    Task<List<Competence>> GetCompetencesAsync(CompetenceQuery query, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> CreateCompetenceAsync(CreateCompetenceCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateCompetenceAsync(UpdateCompetenceCommand command
        , CancellationToken cancellationToken = default);

    Task<ValidationErrors> DeleteCompetenceAsync(DeleteCompetenceCommand command
        , CancellationToken cancellationToken = default);
}