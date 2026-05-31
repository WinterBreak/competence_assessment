using CompetenceAssessment.Core.Models;

namespace CompetenceAssessment.Domain.Assessment;

public interface ITemplateRepository
{
    Task<ITemplate?> GetTemplateAsync(TemplateQuery query, CancellationToken token = default);
    
    Task<List<ITemplate>> GetTemplatesAsync(TemplateQuery query, CancellationToken token = default);
    
    Task<PaginatedResponse<ITemplate>> GetPaginatedTemplatesAsync(TemplateQuery query, CancellationToken token = default);
    
    Task AddTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task UpdateTemplateAsync(ITemplate template, CancellationToken token = default);
    
    Task RemoveTemplateAsync(int id, CancellationToken token = default);
    
    Task SaveAllChanges(CancellationToken token = default);
}