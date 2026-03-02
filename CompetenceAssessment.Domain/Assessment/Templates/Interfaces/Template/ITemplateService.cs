using CompetenceAssessment.Core.Validations;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateService
{
    Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken token = default);
    
    Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken token = default);
    
    Task<ValidationErrors> CreateTemplateAsync(CreateTemplateCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> UpdateTemplateAsync(UpdateTemplateCommand command
        , CancellationToken token = default);
    
    Task<ValidationErrors> DeleteTemplateAsync(DeleteTemplateCommand command
        , CancellationToken token = default);
}