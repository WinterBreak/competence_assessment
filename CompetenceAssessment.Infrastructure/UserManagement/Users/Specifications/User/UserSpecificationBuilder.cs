using CompetenceAssessment.Core.Specifications;
using CompetenceAssessment.Domain.UserManagement;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserSpecificationBuilder: SpecificationBuilderBase<User>
{
    public UserSpecificationBuilder WithQuery(UserQuery query)
    {
        return WithIds(query.Ids)
            .WithRoles(query.RolesIds)
            .WithPosition(query.PositionId)
            .WithDepartment(query.DepartmentId)
            .WithBoss(query.BossId);
    }
    
    public UserSpecificationBuilder WithIds(IEnumerable<int> ids)
    {
        if (ids == null || !ids.Any())
        {
            return this;
        }
        
        var specification = new UserIdSpecification(ids);
        AppendSpecification(specification);
        return this;
    }
    
    public UserSpecificationBuilder WithRoles(IEnumerable<int> roleIds)
    {
        if (roleIds == null || !roleIds.Any())
        {
            return this;
        }
        
        var specification = new UserRolesSpecification(roleIds);
        AppendSpecification(specification);
        return this;
    }

    public UserSpecificationBuilder WithPosition(int positionId)
    {
        if (positionId == null)
        {
            return this;
        }
        
        var specification = new UserPositionSpecification(positionId);
        AppendSpecification(specification);
        return this;
    }
    
    public UserSpecificationBuilder WithDepartment(int departmentId)
    {
        if (departmentId == null)
        {
            return this;
        }
        
        var specification = new UserDepartmentSpecification(departmentId);
        AppendSpecification(specification);
        return this;
    }
    
    public UserSpecificationBuilder WithBoss(int? bossId)
    {
        var specification = new UserBossSpecification(bossId);
        AppendSpecification(specification);
        return this;
    }
}