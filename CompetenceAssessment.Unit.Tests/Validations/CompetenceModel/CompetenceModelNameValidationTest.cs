using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceModelNameValidationTest
{
    [Theory]
    [InlineData("Тестовая модель")]
    [InlineData("Test model")]
    [InlineData("123")]
    public async Task Validate_Name_Is_Valid(string name)
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel(name, null, DateTime.UtcNow
            , new List<CompetenceWeight>());

        var rule = CreateRule(model.Name, false);
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_EmptyName_NotValid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel(string.Empty, null, DateTime.UtcNow
            , new List<CompetenceWeight>());
        
        var rule = CreateRule(model.Name, false);
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_LongName()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel(new string('a', 256), null, DateTime.UtcNow
            , new List<CompetenceWeight>());
        
        var rule = CreateRule(model.Name, false);
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_TakenName()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel("name", null, DateTime.UtcNow
            , new List<CompetenceWeight>());

        var rule = CreateRule(model.Name, true);
        
        await rule.ValidateAsync(model, errors);
        Assert.Equal(true, errors.HasErrors);
    }
    
    private IValidationRule<CompetenceModel> CreateRule(string name, bool result)
    {
        var moq = new Mock<ICompetenceModelValidationQueries>();
        moq.Setup(m => m
                .IsNameTakenAsync(name, CancellationToken.None))
            .ReturnsAsync(result);
        
        return new CompetenceModelNameValidation(moq.Object);
    }
}