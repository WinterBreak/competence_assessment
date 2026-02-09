using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public class ParameterReplacer: ExpressionVisitor
{
    private readonly ParameterExpression _parameter;

    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (_parameter.Type != node.Type)
        {
            return node;
        }
        
        return base.VisitParameter(_parameter);
    }

    internal ParameterReplacer(ParameterExpression parameter)
    {
        _parameter = parameter;
    }
}