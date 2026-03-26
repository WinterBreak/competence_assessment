using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentResultAssessmentSpecification: SpecificationBase<AssessmentResult>
{
    private readonly int _assessmentId = new();

    public AssessmentResultAssessmentSpecification(int assessmentId)
    {
        _assessmentId = assessmentId;
    }
    
    public override Expression<Func<AssessmentResult, bool>> Criteria 
        => t => _assessmentId == t.AssessmentId;
}