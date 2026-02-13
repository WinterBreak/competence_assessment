using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateService
{
    Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken cancellationToken = default);
    
    Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> CreateTemplateAsync(CreateTemplateCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> UpdateTemplateAsync(UpdateTemplateCommand command
        , CancellationToken cancellationToken = default);
    
    Task<ValidationErrors> DeleteTemplateAsync(DeleteTemplateCommand command
        , CancellationToken cancellationToken = default);
}