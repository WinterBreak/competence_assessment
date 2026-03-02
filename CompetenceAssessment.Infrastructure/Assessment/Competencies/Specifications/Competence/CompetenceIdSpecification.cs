using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceIdSpecification: SpecificationBase<Competence>
{
    private readonly List<int> _ids = new();

    public CompetenceIdSpecification(int id)
    {
        _ids.Add(id);
    }

    public CompetenceIdSpecification(IEnumerable<int> ids)
    {
        _ids = ids?.ToList() ?? _ids;
    }
    
    public override Expression<Func<Competence, bool>> Criteria 
        => c => _ids.Contains(c.Id);
}