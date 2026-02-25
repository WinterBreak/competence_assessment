using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceDescriptionTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("описание")]
    public async Task Validate_CompetenceDescription_Is_Valid(string? description)
    {
        var errors = new ValidationErrors();
        var competence = new Competence("ф", description);

        var rule = new CompetenceDescriptionValidation();
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_LongName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence("a", new string('a', 501));
        
        var rule = new CompetenceDescriptionValidation();;
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
}