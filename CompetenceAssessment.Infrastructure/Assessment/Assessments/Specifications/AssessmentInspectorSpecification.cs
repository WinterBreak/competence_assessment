using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentInspectorSpecification: SpecificationBase<Assessment.Assessment>
{
    private readonly List<int> _assessmentIds = new();

    public AssessmentInspectorSpecification(IEnumerable<int> assessmentIds)
    {
        _assessmentIds = assessmentIds?.ToList() ?? _assessmentIds;
    }
    
    public override Expression<Func<Assessment.Assessment, bool>> Criteria 
        => t => !_assessmentIds.Any() || _assessmentIds.Any(a => t.Inspectors.Select(i => i.UserId).Contains(a));
}