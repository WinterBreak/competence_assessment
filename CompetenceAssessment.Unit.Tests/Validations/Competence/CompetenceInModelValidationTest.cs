using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations.Competencies;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceInModelValidationTest
{
    [Fact]
    public async Task Validate_NotInModel_IsValid()
    {
        var errors = new ValidationErrors();
        var competence = new Competence(1,"h", null);

        var rule = CreateRule(competence.Id, false);
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_InModel_NotValid()
    {
        var errors = new ValidationErrors();
        var competence = new Competence(1, "h", null);
        
        var rule = CreateRule(competence.Id, true);
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private IValidationRule<Competence> CreateRule(int id, bool result)
    {
        var moq = new Mock<ICompetenceValidationQueries>();
        moq.Setup(m => m
                .IsUsedInModelsAsync(id, CancellationToken.None))
            .ReturnsAsync(result);
        
        return new CompetenceInModelValidation(moq.Object);
    }
}