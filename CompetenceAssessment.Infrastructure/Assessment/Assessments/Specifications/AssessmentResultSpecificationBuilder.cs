using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentResultSpecificationBuilder: SpecificationBuilderBase<AssessmentResult>
{
    public AssessmentResultSpecificationBuilder WithAssessmentId(int assessmentId)
    {
        var specification = new AssessmentResultAssessmentSpecification(assessmentId);
        AppendSpecification(specification);
        return this;
    }
}