using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Core.Validations;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Validations;

public class UpdateCompetenceTest
{
    [Theory]
    [InlineData("фффф", null)]
    [InlineData("Rjvve", "описание")]
    public async Task Validate_Is_Valid(string name, string? description)
    {
        var command = new UpdateCompetenceCommand(1, name, description);
        var service = CreateService();
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new UpdateCompetenceCommand(1, name, description);
        var service = CreateService();
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new UpdateCompetenceCommand(1, new string('a', 256), null);
        var service = CreateService();
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new UpdateCompetenceCommand(1, "a", new string('a', 501));
        var service = CreateService();
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceService CreateService()
    {
        var repoMoq = new Mock<ICompetenceRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(new Competence(1,"Стрессоустойчивость ", null));
        
        var competencies = Competencies().ToList();
        repoMoq.Setup(m =>
                m.GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(competencies);

        var errors = new ValidationErrors();
        var validationService = new CompetenceValidationService(repoMoq.Object);
        
        return new CompetenceService(repoMoq.Object, validationService);
    }
    
    private static IEnumerable<Competence> Competencies()
    {
        yield return new Competence(2, "Коммуникабельность", null);
        yield return new Competence(1, "Стрессоустойчивость ", null);
    }
}