using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskSpecificationBuilder: SpecificationBuilderBase<Task>
{
    public TaskSpecificationBuilder WithQuery(TaskQuery query)
    {
        return WithIds(query.Ids)
            .WithType(query.Type);
    }
    
    public TaskSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new TaskIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public TaskSpecificationBuilder WithType(TaskType type)
    {
        var specification = new TaskTypeSpecification(type);
        AppendSpecification(specification);
        return this;
    }
}