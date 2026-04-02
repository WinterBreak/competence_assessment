using System.Linq.Expressions;

namespace CompetenceAssessment.Core.Specifications;

public abstract class SpecificationBase<T> : ISpecification<T> where T : class
{
    public abstract Expression<Func<T, bool>> Criteria { get; }
    
    public static implicit operator Expression<Func<T, bool>>
        (SpecificationBase<T> specification) => specification.Criteria;
    
    public static implicit operator Func<T, bool>
        (SpecificationBase<T> specification) => specification.Criteria.Compile();
}