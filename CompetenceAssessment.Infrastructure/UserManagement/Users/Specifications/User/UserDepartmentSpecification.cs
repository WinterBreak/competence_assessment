using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserDepartmentSpecification: SpecificationBase<User>
{
    private readonly int _departmentId = new();

    public UserDepartmentSpecification(int departmentId)
    {
        _departmentId = departmentId;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => u.DepartmentId == _departmentId;
}