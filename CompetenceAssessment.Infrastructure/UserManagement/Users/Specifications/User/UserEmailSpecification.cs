using System.Linq.Expressions;
using CompetenceAssessment.Core.Specifications;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserEmailSpecification: SpecificationBase<User>
{
    private readonly string _email;

    public UserEmailSpecification(string email)
    {
        _email = email;
    }
    
    public override Expression<Func<User, bool>> Criteria 
        => u => u.Email == _email;
}