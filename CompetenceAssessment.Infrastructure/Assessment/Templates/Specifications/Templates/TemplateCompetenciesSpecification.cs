using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TemplateCompetenciesSpecification: SpecificationBase<Template>
{
    private readonly List<int> _competenceIds = new();

    public TemplateCompetenciesSpecification(int competenceId)
    {
        _competenceIds.Add(competenceId);
    }

    public TemplateCompetenciesSpecification(IEnumerable<int> competenceIds)
    {
        _competenceIds = competenceIds?.ToList() ?? _competenceIds;
    }
    
    public override Expression<Func<Template, bool>> Criteria 
        => t => !_competenceIds.Any() || t.TemplateDetails.Any(td => _competenceIds.Contains(td.CompetenceId));
}