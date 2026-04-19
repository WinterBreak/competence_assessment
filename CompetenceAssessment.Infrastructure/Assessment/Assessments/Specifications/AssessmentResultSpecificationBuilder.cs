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
    
    public AssessmentResultSpecificationBuilder WithAssessmentIds(IEnumerable<int> assessmentIds)
    {
        if (assessmentIds == null || !assessmentIds.Any())
        {
            return this;
        }
        
        var specification = new AssessmentResultAssessmentSpecification(assessmentIds);
        AppendSpecification(specification);
        return this;
    }
}