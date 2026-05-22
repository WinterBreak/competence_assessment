using CompetenceAssessment.Application.Assessment;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.UserManagement;
using Moq;
using Xunit;

namespace CompetenceAssessment.Unit.Tests.Analytics;

public class AssessmentAnalyticsServiceTests
{
    private readonly Mock<IAssessmentAnalyticsRepository> _repo;
    private readonly AssessmentAnalyticsService _service;

    public AssessmentAnalyticsServiceTests()
    {
        _repo = new Mock<IAssessmentAnalyticsRepository>();
        _service = new AssessmentAnalyticsService(_repo.Object);
    }

    [Fact]
    public async Task GetParticipantsCompetencies_Should_Set_Level()
    {
        var data = BuildParticipantsData();

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetParticipantsCompetenciesAsync();

        Assert.All(result.SelectMany(x => x.ParticipantCompetences),
            c => Assert.False(string.IsNullOrEmpty(c.Level)));

        Assert.Contains(result.SelectMany(x => x.ParticipantCompetences),
            c => c.Level == "Высокий");
    }

    [Fact]
    public async Task GetPositionsCompetencies_Should_Group_By_Position()
    {
        var data = BuildParticipantsData();

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetPositionsCompetenciesAsync();

        Assert.Single(result);
        Assert.Equal("Dev", result[0].PositionName);
    }

    [Fact]
    public async Task GetPositionsCompetencies_Should_Calculate_Average_Score()
    {
        var data = BuildParticipantsData();

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetPositionsCompetenciesAsync();

        Assert.True(result[0].Score > 0);
        Assert.True(result[0].Percentage > 0);
    }

    [Fact]
    public async Task GetDepartmentCompetencies_Should_Group_By_Department()
    {
        var data = BuildParticipantsData();

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetDepartmentCompetenciesAsync();

        Assert.Single(result);
        Assert.Equal("IT", result[0].DepartmentName);
    }

    [Fact]
    public async Task GetDepartmentCompetencies_Should_Set_Maturity_Level()
    {
        var data = BuildParticipantsData();

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetDepartmentCompetenciesAsync();

        Assert.False(string.IsNullOrEmpty(result[0].Level));
    }

    [Fact]
    public async Task GetCompetenceDevelopment_Should_Call_Repository()
    {
        _repo.Setup(r => r.GetCompetenceDevelopmentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompetenceDevelopmentData());

        var result = await _service.GetCompetenceDevelopmentAsync(1);

        Assert.NotNull(result);

        _repo.Verify(r => r.GetCompetenceDevelopmentAsync(1, It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task GetPositionsCompetencies_Should_Handle_Empty_Data()
    {
        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentParticipantCompetencies>());

        var result = await _service.GetPositionsCompetenciesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    [Fact]
    public async Task GetDepartmentCompetencies_Should_Handle_Empty_Data()
    {
        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentParticipantCompetencies>());

        var result = await _service.GetDepartmentCompetenciesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
    // [Fact]
    // public async Task GetPositionsCompetencies_Should_Return_Null_Group_Handled()
    // {
    //     var data = new List<AssessmentParticipantCompetencies>
    //     {
    //         new AssessmentParticipantCompetencies(
    //             new User
    //             {
    //                 Id = 1,
    //                 PositionId = 10,
    //                 Position = "Dev",
    //                 DepartmentId = 100,
    //                 Department = "IT"
    //             },
    //             new List<ParticipantCompetence>()
    //         )
    //     };
    //
    //     _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(data);
    //
    //     var result = await _service.GetPositionsCompetenciesAsync();
    //
    //     Assert.Empty(result);
    // }
    
    [Fact]
    public async Task GetParticipantsCompetencies_Should_Cover_All_Levels()
    {
        var competence = new Competence { Id = 1, Name = "C1" };

        var user = new User
        {
            Id = 1,
            PositionId = 10,
            Position = "Dev",
            DepartmentId = 100,
            Department = "IT"
        };

        var data = new List<AssessmentParticipantCompetencies>
        {
            new AssessmentParticipantCompetencies(user, new List<ParticipantCompetence>
            {
                new ParticipantCompetence(competence, 10, 10, 95), // Высокий
                new ParticipantCompetence(competence, 10, 10, 75), // Выше среднего
                new ParticipantCompetence(competence, 10, 10, 55), // Средний
                new ParticipantCompetence(competence, 10, 10, 35), // Ниже среднего
                new ParticipantCompetence(competence, 10, 10, 10)  // Низкий
            })
        };

        _repo.Setup(r => r.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(data);

        var result = await _service.GetParticipantsCompetenciesAsync();

        var levels = result.SelectMany(x => x.ParticipantCompetences)
            .Select(x => x.Level)
            .Distinct()
            .ToList();

        Assert.Contains("Высокий", levels);
        Assert.Contains("Выше среднего", levels);
        Assert.Contains("Средний", levels);
        Assert.Contains("Ниже среднего", levels);
        Assert.Contains("Низкий", levels);
    }

    private List<AssessmentParticipantCompetencies> BuildParticipantsData()
    {
        var competence = new Competence { Id = 1, Name = "C1" };

        var user = new User
        {
            Id = 1,
            PositionId = 10,
            Position = "Dev",
            DepartmentId = 100,
            Department = "IT"
        };

        var pc = new ParticipantCompetence(
            competence,
            scale: 10,
            score: 80,
            percentage: 80
        )
        {
            Level = "Средний"
        };

        var pcHigh = new ParticipantCompetence(
            competence,
            scale: 10,
            score: 95,
            percentage: 95
        );

        return new List<AssessmentParticipantCompetencies>
        {
            new AssessmentParticipantCompetencies(
                user,
                new List<ParticipantCompetence> { pc, pcHigh }
            )
        };
    }
}