using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Infrastructure.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentResultAssessmentSpecification: SpecificationBase<AssessmentResult>
{
    private readonly List<int> _assessmentIds = new();

    public AssessmentResultAssessmentSpecification(IEnumerable<int> assessmentIds)
    {
        _assessmentIds = assessmentIds?.ToList() ?? _assessmentIds;
    }
    
    public AssessmentResultAssessmentSpecification(int assessmentId)
    {
        _assessmentIds.Add(assessmentId);
    }
    
    public override Expression<Func<AssessmentResult, bool>> Criteria 
        => t => !_assessmentIds.Any() || _assessmentIds.Contains(t.AssessmentId);
}