using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Competencies;

public class CompetenceModelCompetenceSpecification: SpecificationBase<CompetenceModel>
{
    private readonly List<int> _competenceIds = new();

    public CompetenceModelCompetenceSpecification(IEnumerable<int> competenceIds)
    {
        _competenceIds = competenceIds.ToList() ?? _competenceIds;
    }
    
    public CompetenceModelCompetenceSpecification(int competenceId)
    {
        _competenceIds.Add(competenceId);
    }
    
    public override Expression<Func<CompetenceModel, bool>> Criteria
        => cm => cm.Weights.Select(w => w.CompetenceId).Any(id => _competenceIds.Contains(id));
}