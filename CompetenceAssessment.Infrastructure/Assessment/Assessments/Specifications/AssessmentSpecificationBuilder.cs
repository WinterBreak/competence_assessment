using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentSpecificationBuilder: SpecificationBuilderBase<Assessment.Assessment>
{
    public AssessmentSpecificationBuilder WithQuery(AssessmentQuery query)
    {
        return WithIds(query.Ids)
            .WithCandidates(query.CandidateIds)
            .WithType(query.Type)
            .WithStatus((int)query.State);
    }
    
    public AssessmentSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new AssessmentIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public AssessmentSpecificationBuilder WithCandidates(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new AssessmentCandidateSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public AssessmentSpecificationBuilder WithInspectors(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new AssessmentInspectorSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public AssessmentSpecificationBuilder WithType(AssessmentType type)
    {
        var specification = new AssessmentTypeSpecification(type);
        AppendSpecification(specification);
        return this;
    }
    
    public AssessmentSpecificationBuilder WithStatus(int state)
    {
        if (state == (int)AssessmentState.All)
        {
            return this;
        }
        
        var specification = new AssessmentStateSpecification(state);
        AppendSpecification(specification);
        return this;
    }
}