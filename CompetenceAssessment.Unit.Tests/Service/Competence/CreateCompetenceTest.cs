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

        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
            => q.IsNameTakenAsync(name, null, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, false);
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new CreateCompetenceCommand(name, description);
        
        var service = CreateService(command.Name, false);
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_NameIsTaken_NotValid(string name, string? description)
    {
        var command = new CreateCompetenceCommand(name, description);
        
        var service = CreateService(command.Name, true);
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new CreateCompetenceCommand(new string('a', 256), null);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(command.Name, null, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, false);
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new CreateCompetenceCommand("a", new string('a', 501));
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(command.Name, null, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, false);
        var errors = await service.CreateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceService CreateService(string name, bool queryResult)
    {
        var competencies = Competencies().ToList();
        var repoMoq = new Mock<ICompetenceRepository>();
        repoMoq.Setup(m => m
                .GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(competencies);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(name, null, CancellationToken.None))
            .ReturnsAsync(queryResult);
        
        var validationService = new CompetenceValidationService(queriesMoq.Object);
        
        return new CompetenceService(repoMoq.Object, validationService);
    }

    private static IEnumerable<Competence> Competencies()
    {
        yield return new Competence("Коммуникабельность", null);
        yield return new Competence("Стрессоустойчивость ", null);
    }
}