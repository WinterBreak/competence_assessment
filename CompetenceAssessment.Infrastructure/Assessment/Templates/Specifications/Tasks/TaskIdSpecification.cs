using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.Assessment;

public class TaskIdSpecification: SpecificationBase<Task>
{
    private readonly List<int> _ids = new();

    public TaskIdSpecification(int id)
    {
        _ids.Add(id);
    }

    public TaskIdSpecification(IEnumerable<int> ids)
    {
        _ids = ids?.ToList() ?? _ids;
    }
    
    public override Expression<Func<Task, bool>> Criteria 
        => c => _ids.Contains(c.Id) || !_ids.Any();
}