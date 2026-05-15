using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Service;

public class UpdateCompetenceModelModelTest
{
    [Theory]
    [InlineData("фффф", null)]
    [InlineData("Rjvve", "описание")]
    public async Task Validate_Is_Valid(string name, string? description)
    {
        var command = new UpdateCompetenceModelCommand(1, name, description, WeightDict());
        
        var service = CreateService(command.Name, command.Id, false, false);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(false, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("", null)]
    public async Task Validate_Not_Valid(string name, string? description)
    {
        var command = new UpdateCompetenceModelCommand(1, name, description, WeightDict());
        
        var service = CreateService(command.Name, command.Id, false, false);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Theory]
    [InlineData("Коммуникабельность", null)]
    public async Task Validate_NameIsTaken_NotValid(string name, string? description)
    {
        var command = new UpdateCompetenceModelCommand(1, name, description, WeightDict());
        
        var service = CreateService(command.Name, command.Id,true, true);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongName_Not_Valid()
    {
        var command = new UpdateCompetenceModelCommand(1, new string('a', 256), null, WeightDict());
        
        var service = CreateService(command.Name, command.Id, false, false);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }
    
    [Fact]
    public async Task Validate_LongDescr_Not_Valid()
    {
        var command = new UpdateCompetenceModelCommand(1, "a", new string('a', 1001), WeightDict());
        
        var service = CreateService(command.Name, command.Id, false, false);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    [Fact]
    public async Task Validate_NoWeights_Invalid()
    {
        var command = new UpdateCompetenceModelCommand(1, "a", new string('a', 999)
            , new Dictionary<int, decimal>());
        
        var service = CreateService(command.Name, command.Id, false, false);
        var errors = await service.UpdateCompetenceModelAsync(command, CancellationToken.None);
        
        Assert.Equal(true, errors.HasErrors);
    }

    private CompetenceModelService CreateService(string name, int id, bool isUsed, bool isTaken)
    {
        var repoMoq = new Mock<ICompetenceModelRepository>();
        repoMoq.Setup(m => m
                .GetCompetenceModelAsync(It.IsAny<CompetenceModelQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompetenceModel(1,"Стрессоустойчивость ", null, DateTime.Now
                ,ValidWeights().ToList() ));
        
        var models = Models().ToList();
        repoMoq.Setup(m =>
                m.GetCompetenceModelsAsync(It.IsAny<CompetenceModelQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(models);
        
        var queriesMoq = new Mock<ICompetenceModelValidationQueries>();
        queriesMoq.Setup(q 
                => q.IsModelExist(It.IsAny<int>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        queriesMoq.Setup(q 
                => q.IsNameTakenAsync(It.IsAny<string>(),It.IsAny<int>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(isTaken);
        queriesMoq.Setup(q 
                => q.IsUsedInTemplateAsync(It.IsAny<int>()
                    , It.IsAny<CancellationToken>()))
            .ReturnsAsync(isUsed);
        
        var validationService = new CompetenceModelValidationService(queriesMoq.Object);
        
        return new CompetenceModelService(repoMoq.Object, validationService);
    }
    
    private static IEnumerable<CompetenceModel> Models()
    {
        yield return new CompetenceModel(2, "Коммуникабельность", null, DateTime.Now
            ,ValidWeights().ToList());
        yield return new CompetenceModel(1, "Стрессоустойчивость ", null, DateTime.Now
            ,ValidWeights().ToList());
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