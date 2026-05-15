using CompetenceAssessment.Core.Specifications;
using Xunit;

namespace CompetenceAssessment.Tests.Core.Specifications;

public class NotSpecificationTests
{
    // [Fact]
    // public void NotSpecification_Should_Invert_Condition()
    // {
    //     var inner = new TestSpecification(x => x.IsActive);
    //
    //     var specification = new NotSpecification<TestEntity>(inner);
    //
    //     var predicate = specification.Criteria.Compile();
    //
    //     Assert.False(predicate(new TestEntity
    //     {
    //         IsActive = true
    //     }));
    //
    //     Assert.True(predicate(new TestEntity
    //     {
    //         IsActive = false
    //     }));
    // }

    private class TestEntity
    {
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