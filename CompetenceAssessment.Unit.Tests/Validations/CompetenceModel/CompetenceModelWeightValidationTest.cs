using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceModelWeightValidationTest
{
    [Fact]
    public async Task Validate_Weights_Is_Valid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel("name", null, DateTime.UtcNow
            , ValidWeights().ToList());
        
        var rule = new CompetenceModelWeightValidation();
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_RepeatingCompetencies_NotValid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel("name", null, DateTime.UtcNow
            , InvalidWeights().ToList());
        
        var rule = new CompetenceModelWeightValidation();
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_NoWeights_NotValid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel("name", null, DateTime.UtcNow
            , new List<CompetenceWeight>());
        
        var rule = new CompetenceModelWeightValidation();
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private static IEnumerable<CompetenceWeight> ValidWeights()
    {
        yield return new CompetenceWeight(new Competence(1, "f", ""), 1);
        yield return new CompetenceWeight(new Competence(2, "у", ""), 1);
        yield return new CompetenceWeight(new Competence(3, "d", ""), 1);
    }
    
    private static IEnumerable<CompetenceWeight> InvalidWeights()
    {
        yield return new CompetenceWeight(new Competence(1, "f", ""), 1);
        yield return new CompetenceWeight(new Competence(2, "у", ""), 1);
        yield return new CompetenceWeight(new Competence(2, "d", ""), 1);
    }
}