using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public class NotSpecification<T> : SpecificationBase<T> where T : class
{
    private readonly Expression<Func<T, bool>> _criteria;

    public NotSpecification(ISpecification<T> specification)
    {
        var parameterExpression = Expression.Parameter(typeof(T));
        var expressionBody = Expression.Not(specification.Criteria.Body);

        _criteria = Expression.Lambda<Func<T, bool>>(expressionBody, parameterExpression);
    }

    public override Expression<Func<T, bool>> Criteria => _criteria;
}