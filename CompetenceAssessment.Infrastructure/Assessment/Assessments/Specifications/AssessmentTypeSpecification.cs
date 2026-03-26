using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentTypeSpecification: SpecificationBase<Assessment.Assessment>
{
    private readonly AssessmentType _type;

    public AssessmentTypeSpecification(AssessmentType type)
    {
        _type = type;
    }

    public override Expression<Func<Assessment.Assessment, bool>> Criteria
        => c => c.AssessmentTypeId == (int)_type || _type == AssessmentType.None;
}