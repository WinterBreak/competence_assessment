using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentIsFinishedSpecification: SpecificationBase<Assessment.Assessment>
{
    private readonly bool _isFinished;

    public AssessmentIsFinishedSpecification(bool isFinished)
    {
        _isFinished = isFinished;
    }

    public override Expression<Func<Assessment.Assessment, bool>> Criteria
        => c => c.IsFinished == _isFinished;
}