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

        var rule = CreateRule();
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(false, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_CompetenceName_EmptyName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence("", null);
        
        var rule = CreateRule();
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_LongName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence(new string('a', 256), null);
        
        var rule = CreateRule();
        await rule.ValidateAsync(competence, errors);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_CompetenceName_TakenName()
    {
        var errors = new ValidationErrors();
        var competence = new Competence("Коммуникабельность", null);
        
        var moq = new Mock<ICompetenceRepository>();
        var competencies = CompetenciesWithTakenName().ToList();
        moq.Setup(m => m
                .GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(competencies);
        var rule = new CompetenceNameValidation(moq.Object);
        
        await rule.ValidateAsync(competence, errors);
        Assert.Equal(true, errors.HasErrors);
    }

    private IValidationRule<Competence> CreateRule()
    {
        var moq = new Mock<ICompetenceRepository>();
        moq.Setup(m => m
                .GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<Competence> ());
        
        return new CompetenceNameValidation(moq.Object);
    }

    private static IEnumerable<Competence> CompetenciesWithTakenName()
    {
        yield return new Competence("Коммуникабельность", null);
        yield return new Competence("Стрессоустойчивость ", null);
    }
}