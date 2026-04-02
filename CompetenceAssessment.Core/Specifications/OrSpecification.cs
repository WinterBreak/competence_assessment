using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public class OrSpecification<T> : SpecificationBase<T> where T : class
{
    private readonly Expression<Func<T, bool>> _criteria;

    public OrSpecification(ISpecification<T> leftSpecification
        , ISpecification<T> rightSpecification)
    {
        var leftExpression = leftSpecification.Criteria;
        var rightExpression = rightSpecification.Criteria;

        var parameterExpression = Expression.Parameter(typeof(T));
        var expressionBody = Expression.OrElse(leftExpression.Body, rightExpression.Body);
        expressionBody = (BinaryExpression)new ParameterReplacer(parameterExpression)
            .Visit(expressionBody);
        _criteria = Expression.Lambda<Func<T, bool>>(expressionBody, parameterExpression);
    }

    public override Expression<Func<T, bool>> Criteria => _criteria;
}