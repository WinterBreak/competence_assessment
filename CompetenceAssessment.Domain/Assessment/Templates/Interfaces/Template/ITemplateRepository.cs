namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateRepository
{
    Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken cancellationToken = default);
    
    Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken cancellationToken = default);
    
    Task AddTemplateAsync(ITemplate template, CancellationToken cancellationToken = default);
    
    Task UpdateTemplateAsync(ITemplate template, CancellationToken cancellationToken = default);
    
    Task RemoveTemplateAsync(ITemplate template, CancellationToken cancellationToken = default);
    
    Task SaveAllChanges(CancellationToken cancellationToken = default);
}