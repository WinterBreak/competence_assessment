using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceModelDescriptionTest
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("описание")]
    public async Task Validate_CompetenceModelDescription_Is_Valid(string? description)
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel("ф", description, DateTime.UtcNow
            , new List<CompetenceWeight> ());

        var rule = new CompetenceModelDescriptionValidation();
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceModelName_LongName()
    {
        var errors = new ValidationErrors();
        var competence = new CompetenceModel("a", new string('a', 1001), DateTime.UtcNow
            , new List<CompetenceWeight>());
        
        var rule = new CompetenceModelDescriptionValidation();;
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
}