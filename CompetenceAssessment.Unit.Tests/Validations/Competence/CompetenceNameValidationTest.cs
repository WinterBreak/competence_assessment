using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Assessment.Competencies.Validations;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CompetenceNameValidationTest
{
    [Fact]
    public async Task Validate_CompetenceName_Is_Valid()
    {
        const string CONST_NAME = "Коммуникабельность";

        var errors = new ValidationErrors();
        var competence = new Competence(CONST_NAME, null);

        var rule = CreateRule(competence.Name, null, false);
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_CompetenceName_EmptyName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence("", null);
        
        var rule = CreateRule(competence.Name, null, false);
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_LongName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence(new string('a', 256), null);
        
        var rule = CreateRule(competence.Name,null,false);
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_TakenName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence("Коммуникабельность", null);

        var rule = CreateRule(competence.Name, null, true);
        
        await rule.ValidateAsync(competence, errors);
        Assert.Equal(true, errors.HasErrors);
    }

    private IValidationRule<Competence> CreateRule(string name, int? id, bool result)
    {
        var moq = new Mock<ICompetenceValidationQueries>();
        moq.Setup(m => m
                .IsNameTakenAsync(name, id, CancellationToken.None))
            .ReturnsAsync(result);
        
        return new CompetenceNameValidation(moq.Object);
    }
}