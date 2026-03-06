using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.Assessment;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskTypeSpecification : SpecificationBase<Task>
{
    private readonly TaskType _type;

    public TaskTypeSpecification(TaskType type)
    {
        _type = type;
    }

    public override Expression<Func<Task, bool>> Criteria
        => c => c.TaskTypeId == (int)_type || _type == TaskType.None;
}