using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateIdSpecification: SpecificationBase<Template>
{
    private readonly List<int> _ids = new();

    public TemplateIdSpecification(int id)
    {
        _ids.Add(id);
    }

    public TemplateIdSpecification(IEnumerable<int> ids)
    {
        _ids = ids?.ToList() ?? _ids;
    }
    
    public override Expression<Func<Template, bool>> Criteria 
        => t => _ids.Contains(t.Id) || !_ids.Any();
}