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
    {
        get
        {
            if (!_names.Any())
            {
                return c => true;
            }

            return c => _names.Contains(c.Name);
        }
    }
}