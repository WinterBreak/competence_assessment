using CompetenceAssessment.Core.Specifications;
using Xunit;

namespace CompetenceAssessment.Tests.Core.Specifications;

public class AndSpecificationTests
{
    [Fact]
    public void AndSpecification_Should_Combine_Conditions()
    {
        var left = new TestSpecification(x => x.Age >= 18);
        var right = new TestSpecification(x => x.IsActive);

        var specification = new AndSpecification<TestEntity>(left, right);

        var predicate = specification.Criteria.Compile();

        Assert.True(predicate(new TestEntity
        {
            Age = 20,
            IsActive = true
        }));

        Assert.False(predicate(new TestEntity
        {
            Age = 15,
            IsActive = true
        }));

        Assert.False(predicate(new TestEntity
        {
            Age = 20,
            IsActive = false
        }));
    }

    private class TestEntity
    {
        public int Age { get; set; }

        public bool IsActive { get; set; }
    }

    private class TestSpecification : SpecificationBase<TestEntity>
    {
        private readonly System.Linq.Expressions.Expression<Func<TestEntity, bool>> _criteria;

        public TestSpecification(System.Linq.Expressions.Expression<Func<TestEntity, bool>> criteria)
        {
            _criteria = criteria;
        }

        public override System.Linq.Expressions.Expression<Func<TestEntity, bool>> Criteria => _criteria;
    }
}