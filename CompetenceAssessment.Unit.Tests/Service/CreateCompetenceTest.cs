using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class CreateCompetenceTest
{
    [Theory]
    [InlineData("фффф", null)]
    [InlineData("Rjvve", "описание")]
    public async Task Validate_Is_Valid(string name, string? description)
    {
        var command = new CreateCompetenceCommand(name, description);
        var service = CreateService();
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new CreateCompetenceCommand(name, description);
        var service = CreateService();
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new CreateCompetenceCommand(new string('a', 256), null);
        var service = CreateService();
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new CreateCompetenceCommand("a", new string('a', 501));
        var service = CreateService();
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceService CreateService()
    {
        var competencies = Competencies().ToList();
        var repoMoq = new Mock<ICompetenceRepository>();
        repoMoq.Setup(m => m
                .GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(competencies);

        var errors = new ValidationErrors();
        var validationService = new CompetenceValidationService(repoMoq.Object);
        
        return new CompetenceService(repoMoq.Object, validationService);
    }

    private static IEnumerable<Competence> Competencies()
    {
        yield return new Competence("Коммуникабельность", null);
        yield return new Competence("Стрессоустойчивость ", null);
    }
}