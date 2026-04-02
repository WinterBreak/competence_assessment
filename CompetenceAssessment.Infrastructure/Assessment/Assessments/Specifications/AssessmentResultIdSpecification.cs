using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentResultIdSpecification: SpecificationBase<AssessmentResult>
{
    private readonly List<int> _resultIds = new();

    public AssessmentResultIdSpecification(int assessmentId)
    {
        _resultIds.Add(assessmentId);
    }

    public AssessmentResultIdSpecification(IEnumerable<int> assessmentIds)
    {
        _resultIds = assessmentIds?.ToList() ?? _resultIds;
    }
    
    public override Expression<Func<AssessmentResult, bool>> Criteria 
        => t => !_resultIds.Any() || _resultIds.Contains(t.Id);
}