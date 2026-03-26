using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentIdSpecification: SpecificationBase<Assessment.Assessment>
{
    private readonly List<int> _assessmentIds = new();

    public AssessmentIdSpecification(int assessmentId)
    {
        _assessmentIds.Add(assessmentId);
    }

    public AssessmentIdSpecification(IEnumerable<int> assessmentIds)
    {
        _assessmentIds = assessmentIds?.ToList() ?? _assessmentIds;
    }
    
    public override Expression<Func<Assessment.Assessment, bool>> Criteria 
        => t => !_assessmentIds.Any() || _assessmentIds.Contains(t.Id);
}