using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserPositionSpecification: SpecificationBase<User>
{
    private readonly int _positionId = new();

    public UserPositionSpecification(int positionId)
    {
        _positionId = positionId;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => u.PositionId == _positionId;
}