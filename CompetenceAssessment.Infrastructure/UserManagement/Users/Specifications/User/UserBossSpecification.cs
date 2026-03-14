using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserBossSpecification: SpecificationBase<User>
{
    private readonly int? _bossId = new();

    public UserBossSpecification(int? bossId)
    {
        _bossId = bossId;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => u.BossId == _bossId;
}