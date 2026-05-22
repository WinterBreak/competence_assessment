using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class TemplateNameValidationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_Should_Add_Error_When_Name_Is_Empty(string? name)
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Name = name
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsNameTakenAsync(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TemplateNameValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Name_Too_Long()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Name = new string('a', 256)
        };

        var queries = new Mock<ITemplateValidationQueries>();

        var rule = new TemplateNameValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Add_Error_When_Name_Is_Already_Taken()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Name = "Template"
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsNameTakenAsync(
                template.Name,
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var rule = new TemplateNameValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.True(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Not_Add_Error_When_Name_Is_Valid_And_Unique()
    {
        var errors = new ValidationErrors();

        var template = new ITemplate
        {
            Id = 1,
            Name = "Template"
        };

        var queries = new Mock<ITemplateValidationQueries>();

        queries.Setup(q => q.IsNameTakenAsync(
                template.Name,
                template.Id,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var rule = new TemplateNameValidation(queries.Object);

        await rule.ValidateAsync(template, errors);

        Assert.False(errors.HasErrors);
    }

    [Fact]
    public async Task Validate_Should_Throw_When_Template_Is_Null()
    {
        var errors = new ValidationErrors();

        var queries = new Mock<ITemplateValidationQueries>();

        var rule = new TemplateNameValidation(queries.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            rule.ValidateAsync(null!, errors));
    }

    [Fact]
    public async Task Validate_Should_Throw_When_ValidationErrors_Is_Null()
    {
        var template = new ITemplate
        {
            Id = 1,
            Name = "Template"
        };

        var queries = new Mock<ITemplateValidationQueries>();

        var rule = new TemplateNameValidation(queries.Object);

        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            rule.ValidateAsync(template, null!));
    }
}