using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public interface ISpecification<T> where T : class
{
    Expression<Func<T, bool>> Criteria { get; }
}