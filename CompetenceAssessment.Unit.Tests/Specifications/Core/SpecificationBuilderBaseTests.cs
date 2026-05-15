using CompetenceAssessment.Core.Specifications;
using Xunit;

namespace CompetenceAssessment.Tests.Core.Specifications;

public class SpecificationBuilderBaseTests
{
    [Fact]
    public void Builder_Should_Combine_With_And_By_Default()
    {
        var builder = new TestBuilder();

        builder.Append(new TestSpecification(x => x.Age >= 18));
        builder.Append(new TestSpecification(x => x.IsActive));

        var predicate = builder.Build().Criteria.Compile();

        Assert.True(predicate(new TestEntity
        {
            Age = 20,
            IsActive = true
        }));

        Assert.False(predicate(new TestEntity
        {
            Age = 20,
            IsActive = false
        }));
    }

    [Fact]
    public void Builder_Should_Combine_With_Or()
    {
        var builder = new TestBuilder();

        builder.Append(new TestSpecification(x => x.Age >= 18));

        builder.Or();

        builder.Append(new TestSpecification(x => x.IsActive));

        var predicate = builder.Build().Criteria.Compile();

        Assert.True(predicate(new TestEntity
        {
            Age = 20,
            IsActive = false
        }));

        Assert.True(predicate(new TestEntity
        {
            Age = 10,
            IsActive = true
        }));

        Assert.False(predicate(new TestEntity
        {
            Age = 10,
            IsActive = false
        }));
    }

    // [Fact]
    // public void Builder_Should_Apply_Not()
    // {
    //     var builder = new TestBuilder();
    //
    //     builder.Not();
    //
    //     builder.Append(new TestSpecification(x => x.IsActive));
    //
    //     var predicate = builder.Build().Criteria.Compile();
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

    private class TestBuilder : SpecificationBuilderBase<TestEntity>
    {
        public void Append(SpecificationBase<TestEntity> specification)
        {
            AppendSpecification(specification);
        }
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