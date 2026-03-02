using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class CompetenceNameSpecification: SpecificationBase<Competence>
{
    private readonly List<string> _names = new();

    public CompetenceNameSpecification(IEnumerable<string> names)
    {
        _names = names?.ToList() ?? _names;
    }

    public CompetenceNameSpecification(string name)
    {
        _names.Add(name);
    }

    public override Expression<Func<Competence, bool>> Criteria
     => c => _names.Contains(c.Name);
}