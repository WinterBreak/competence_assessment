using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Competencies;

public class CompetenceModelNameSpecification: SpecificationBase<CompetenceModel>
{
    private readonly List<string> _names = new();

    public CompetenceModelNameSpecification(IEnumerable<string> names)
    {
        _names = names?.ToList() ?? _names;
    }

    public CompetenceModelNameSpecification(string name)
    {
        _names.Add(name);
    }

    public override Expression<Func<CompetenceModel, bool>> Criteria
        => c => _names.Contains(c.Name);
}