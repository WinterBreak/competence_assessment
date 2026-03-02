using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations.Competencies;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceModelInTemplateValidationTest
{
    [Fact]
    public async Task Validate_NotInTemplate_IsValid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel(1,"h", null, DateTime.Now
            , new List<CompetenceWeight>());

        var rule = CreateRule(model.Id, false);
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_InTemplate_NotValid()
    {
        var errors = new ValidationErrors();
        var model = new CompetenceModel(1,"h", null, DateTime.Now, new List<CompetenceWeight>());
        
        var rule = CreateRule(model.Id, true);
        await rule.ValidateAsync(model, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private IValidationRule<CompetenceModel> CreateRule(int id, bool result)
    {
        var moq = new Mock<ICompetenceModelValidationQueries>();
        moq.Setup(m => m
                .IsUsedInTemplateAsync(id, CancellationToken.None))
            .ReturnsAsync(result);
        
        return new CompetenceModelInTemplateValidation(moq.Object);
    }
}