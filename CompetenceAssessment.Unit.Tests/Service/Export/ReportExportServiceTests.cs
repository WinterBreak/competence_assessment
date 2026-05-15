using ClosedXML.Excel;
using CompetenceAssessment.Application.Export;
using CompetenceAssessment.Domain.Assessment;
using CompetenceAssessment.Domain.Export.Enumerations;
using CompetenceAssessment.Domain.Export.Models;
using CompetenceAssessment.Domain.UserManagement;
using Moq;
using Xunit;

namespace CompetenceAssessment.Tests.Application.Export;

public class ReportExportServiceTests
{
    private readonly Mock<IAssessmentAnalyticsService> _analyticsMock;
    private readonly ReportExportService _service;

    public ReportExportServiceTests()
    {
        _analyticsMock = new Mock<IAssessmentAnalyticsService>();
        _service = new ReportExportService(_analyticsMock.Object);
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Throw_For_Unsupported_Report()
    {
        var request = new FakeReportRequest();

        await Assert.ThrowsAsync<NotSupportedException>(() =>
            _service.GenerateReportAsync(request));
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Generate_Heatmap_Report()
    {
        var participants = CreateParticipants();

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var request = new HeatmapReportRequest();

        var result = await _service.GenerateReportAsync(request);

        Assert.NotNull(result);
        Assert.NotEmpty(result);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        Assert.NotNull(workbook.Worksheet("Тепловая карта"));
        Assert.NotNull(workbook.Worksheet("Данные"));
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Filter_Heatmap_By_Department()
    {
        var participants = CreateParticipants();

        participants[1].User.DepartmentId = 999;

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var request = new HeatmapReportRequest
        {
            DepartmentId = 1
        };

        var result = await _service.GenerateReportAsync(request);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        var sheet = workbook.Worksheet("Тепловая карта");

        Assert.Equal(" John Doe\nDeveloper", sheet.Cell(2, 1).GetString());
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Generate_Position_Matrix_Report()
    {
        _analyticsMock
            .Setup(x => x.GetPositionsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreatePositions());

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateParticipants());

        var request = new PositionMatrixReportRequest();

        var result = await _service.GenerateReportAsync(request);

        Assert.NotEmpty(result);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        Assert.NotNull(workbook.Worksheet("Gap-анализ"));
        Assert.NotNull(workbook.Worksheet("Соответствие сотрудников"));
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Throw_When_Position_Not_Found()
    {
        _analyticsMock
            .Setup(x => x.GetPositionsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PositionCompetencies>());

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateParticipants());

        var request = new PositionMatrixReportRequest();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GenerateReportAsync(request));
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Generate_Organizational_Maturity_Report()
    {
        _analyticsMock
            .Setup(x => x.GetDepartmentCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateDepartments());

        var request = new OrganizationalMaturityReportRequest();

        var result = await _service.GenerateReportAsync(request);

        Assert.NotEmpty(result);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        Assert.NotNull(workbook.Worksheet("Сводка зрелости"));
        Assert.NotNull(workbook.Worksheet("Детализация"));
        Assert.NotNull(workbook.Worksheet("Рекомендации"));
    }

    [Fact]
    public async Task GenerateReportAsync_Should_Throw_When_Department_Not_Found()
    {
        _analyticsMock
            .Setup(x => x.GetDepartmentCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DepartmentCompetencies>());

        var request = new OrganizationalMaturityReportRequest();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.GenerateReportAsync(request));
    }

    [Fact]
    public async Task Heatmap_Report_Should_Contain_Average_Score()
    {
        var participants = CreateParticipants();

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var request = new HeatmapReportRequest();

        var result = await _service.GenerateReportAsync(request);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        var sheet = workbook.Worksheet("Тепловая карта");

        Assert.NotEmpty(sheet.Cell(2, 4).GetString());
    }

    [Fact]
    public async Task Data_Sheet_Should_Contain_Competence_Data()
    {
        var participants = CreateParticipants();

        _analyticsMock
            .Setup(x => x.GetParticipantsCompetenciesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(participants);

        var request = new HeatmapReportRequest();

        var result = await _service.GenerateReportAsync(request);

        using var stream = new MemoryStream(result);
        using var workbook = new XLWorkbook(stream);

        var sheet = workbook.Worksheet("Данные");

        Assert.Equal("EmployeeId", sheet.Cell(1, 1).GetString());
        Assert.Equal("Backend", sheet.Cell(2, 5).GetString());
    }

    private static List<AssessmentParticipantCompetencies> CreateParticipants()
    {
        return
        [
            new AssessmentParticipantCompetencies
            {
                User = new User
                {
                    Id = 1,
                    FirstName = "John Doe",
                    Position = "Developer",
                    Department = "IT",
                    DepartmentId = 1
                },
                ParticipantCompetences =
                [
                    new ParticipantCompetence
                    {
                        Percentage = 85,
                        Score = 17,
                        Scale = 20,
                        Competence = new Competence
                        {
                            Id = 1,
                            Name = "Backend"
                        }
                    },
                    new ParticipantCompetence
                    {
                        Percentage = 50,
                        Score = 10,
                        Scale = 20,
                        Competence = new Competence
                        {
                            Id = 2,
                            Name = "Frontend"
                        }
                    }
                ]
            },
            new AssessmentParticipantCompetencies
            {
                User = new User
                {
                    Id = 2,
                    FirstName = "Jane Smith",
                    Position = "QA",
                    Department = "QA",
                    DepartmentId = 2
                },
                ParticipantCompetences =
                [
                    new ParticipantCompetence
                    {
                        Percentage = 30,
                        Score = 6,
                        Scale = 20,
                        Competence = new Competence
                        {
                            Id = 1,
                            Name = "Backend"
                        }
                    }
                ]
            }
        ];
    }

    private static List<PositionCompetencies> CreatePositions()
    {
        return
        [
            new PositionCompetencies
            {
                PositionId = 1,
                PositionName = "Developer",
                Percentage = 75,
                Level = "Middle",
                Employees =
                [
                    new User
                    {
                        Id = 1,
                        FirstName = "John Doe",
                        Position = "Developer"
                    }
                ],
                Competencies =
                [
                    new CompetenceResult
                    {
                        Percentage = 75,
                        Competence = new Competence
                        {
                            Id = 1,
                            Name = "Backend"
                        }
                    }
                ]
            }
        ];
    }

    private static List<DepartmentCompetencies> CreateDepartments()
    {
        return
        [
            new DepartmentCompetencies
            {
                DepartmentId = 1,
                DepartmentName = "IT",
                Percentage = 72,
                Employees =
                [
                    new User
                    {
                        Id = 1,
                        FirstName = "John Doe"
                    }
                ],
                Competencies =
                [
                    new CompetenceResult
                    {
                        Percentage = 72,
                        Competence = new Competence
                        {
                            Id = 1,
                            Name = "Backend"
                        }
                    }
                ]
            }
        ];
    }

    private class FakeReportRequest : ReportRequest
    {
    }
}