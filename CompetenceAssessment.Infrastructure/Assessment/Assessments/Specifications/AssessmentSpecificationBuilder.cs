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
            .WithStatus(query.IsFinished);
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
    
    public AssessmentSpecificationBuilder WithType(AssessmentType type)
    {
        var specification = new AssessmentTypeSpecification(type);
        AppendSpecification(specification);
        return this;
    }

    // TODO для фильтрации еще нужен вариант "все". enum?
    public AssessmentSpecificationBuilder WithStatus(bool? isFinished)
    {
        if (isFinished is null)
        {
            return this;
        }
        
        var specification = new AssessmentIsFinishedSpecification(isFinished.Value);
        AppendSpecification(specification);
        return this;
    }
}