using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Service;

public class CreateCompetenceModelTest
{
    [Theory]
    [InlineData("фффф", null)]
    [InlineData("Rjvve", "описание")]
    public async Task Validate_Is_Valid(string name, string? description)
    {
        var command = new CreateCompetenceModelCommand(name, WeightDict(), description);
        
        var service = CreateService(command.Name, null, false);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new CreateCompetenceModelCommand(name, WeightDict(), description);
        
        var service = CreateService(command.Name, null, false);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_NameIsTaken_NotValid(string name, string? description)
    {
        var command = new CreateCompetenceModelCommand(name, WeightDict(), description);
        
        var service = CreateService(command.Name, null, true);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new CreateCompetenceModelCommand(new string('a', 256)
            ,  WeightDict(), null);
        
        var service = CreateService(command.Name, null, false);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new CreateCompetenceModelCommand("a", WeightDict()
            , new string('a', 1001));
        
        var service = CreateService(command.Name,  null,false);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_NoWeights_Not_Valid()
    {
        var command = new CreateCompetenceModelCommand("a", new Dictionary<int, decimal>()
            , new string('a', 500));
        
        var service = CreateService(command.Name, null, false);
        var errors = await service.CreateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceModelService CreateService(string name, int? id, bool queryResult)
    {
        var repoMoq = new Mock<ICompetenceModelRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceModelsAsync(It.IsAny<CompetenceModelQuery>(), CancellationToken.None))
            .ReturnsAsync(new List<CompetenceModel>());
        
        var queriesMoq = new Mock<ICompetenceModelValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(It.IsAny<string>(), It.IsAny<int?>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(queryResult);
        
        var validationService = new CompetenceModelValidationService(queriesMoq.Object);
        
        return new CompetenceModelService(repoMoq.Object, validationService);
    }
    
    private static IEnumerable<CompetenceWeight> ValidWeights()
    {
        yield return new CompetenceWeight(new Competence(1, "f", ""), 1);
        yield return new CompetenceWeight(new Competence(2, "у", ""), 1);
        yield return new CompetenceWeight(new Competence(3, "d", ""), 1);
    }

    private static Dictionary<int, decimal> WeightDict() => new()
    {
        [1] = 1m,
        [2] = 1m,
        [3] = 1m
    };
}