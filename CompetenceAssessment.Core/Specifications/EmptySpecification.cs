using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public class EmptySpecification<T>: SpecificationBase<T> where T : class
{
    public override Expression<Func<T, bool>> Criteria => t => true;
}