using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Competencies;

public class CompetenceModelIdSpecification: SpecificationBase<CompetenceModel>
{
    private readonly List<int> _ids = new();

    public CompetenceModelIdSpecification(int id)
    {
        _ids.Add(id);
    }

    public CompetenceModelIdSpecification(IEnumerable<int> ids)
    {
        _ids = ids?.ToList() ?? _ids;
    }
    
    public override Expression<Func<CompetenceModel, bool>> Criteria 
        => c => _ids.Contains(c.Id) || !_ids.Any();
    
}