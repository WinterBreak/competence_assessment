using CompetenceAssessment.Application.Assessment;
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
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(name, command.Id, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, command.Id, false);
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new UpdateCompetenceCommand(1, name, description);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(name, command.Id, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, command.Id, false);
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_NameIsTaken_NotValid(string name, string? description)
    {
        var command = new UpdateCompetenceCommand(1, name, description);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(name, command.Id, CancellationToken.None))
            .ReturnsAsync(true);
        
        var service = CreateService(command.Name, command.Id, true);
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new UpdateCompetenceCommand(1, new string('a', 256), null);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(command.Name, command.Id, CancellationToken.None))
            .ReturnsAsync(false);
        
        var service = CreateService(command.Name, command.Id, false);
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new UpdateCompetenceCommand(1, "a", new string('a', 501));
        
        var service = CreateService(command.Name, command.Id, false);
        var errors = await service.UpdateCompetenceAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceService CreateService(string name, int? id, bool queryResult)
    {
        var repoMoq = new Mock<ICompetenceRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(new Competence(1,"Стрессоустойчивость ", null));
        
        var competencies = Competencies().ToList();
        repoMoq.Setup(m =>
                m.GetCompetenciesAsync(It.IsAny<CompetenceQuery>(), CancellationToken.None))
            .ReturnsAsync(competencies);
        
        var queriesMoq = new Mock<ICompetenceValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(name, id, CancellationToken.None))
            .ReturnsAsync(queryResult);
        
        var validationService = new CompetenceValidationService(queriesMoq.Object);
        
        return new CompetenceService(repoMoq.Object, validationService);
    }
    
    private static IEnumerable<Competence> Competencies()
    {
        yield return new Competence(2, "Коммуникабельность", null);
        yield return new Competence(1, "Стрессоустойчивость ", null);
    }
}