using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessments;

public class AssessmentSpecificationBuilder: SpecificationBuilderBase<Assessment.Assessment>
{
    public AssessmentSpecificationBuilder WithQuery(AssessmentQuery query)
    {
        return WithIds(query.Ids)
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
    
    public AssessmentSpecificationBuilder WithType(AssessmentType type)
    {
        var specification = new AssessmentTypeSpecification(type);
        AppendSpecification(specification);
        return this;
    }

    // TODO для фильтрации еще нужен вариант "все". enum?
    public AssessmentSpecificationBuilder WithStatus(bool isFinished)
    {
        var specification = new AssessmentIsFinishedSpecification(isFinished);
        AppendSpecification(specification);
        return this;
    }
}