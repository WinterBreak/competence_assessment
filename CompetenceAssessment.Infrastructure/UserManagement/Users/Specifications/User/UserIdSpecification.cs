using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserIdSpecification: SpecificationBase<User>
{
    private readonly List<int> _ids = new();

    public UserIdSpecification(int id)
    {
        _ids.Add(id);
    }

    public UserIdSpecification(IEnumerable<int> ids)
    {
        _ids = ids?.ToList() ?? _ids;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => _ids.Contains(u.Id) || !_ids.Any();
}