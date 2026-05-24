using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentStateSpecification: SpecificationBase<Assessment.Assessment>
{
    private readonly int _state;

    public AssessmentStateSpecification(int state)
    {
        _state = state;
    }

    public override Expression<Func<Assessment.Assessment, bool>> Criteria
        => c => _state == (int)AssessmentState.All || c.State == _state;
}