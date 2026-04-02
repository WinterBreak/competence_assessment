using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserRolesSpecification: SpecificationBase<User>
{
    private readonly List<int> _roleIds = new();

    public UserRolesSpecification(int roleId)
    {
        _roleIds.Add(roleId);
    }

    public UserRolesSpecification(IEnumerable<int> roleIds)
    {
        _roleIds = roleIds?.ToList() ?? _roleIds;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => !_roleIds.Any() || u.RoleLinks.Select(r => r.RoleId).Intersect(_roleIds).Any();
}