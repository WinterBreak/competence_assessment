using System.Net;
using System.Net.Http.Json;
using CompetenceAssessment.Domain.Export.Enumerations;
using Xunit;

namespace CompetenceAssessment.IntegrationTests.Reports;

public class ReportExportTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public ReportExportTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Heatmap_Report_Should_Return_Excel_File()
    {
        var request = new
        {
            reportType = ReportType.EmployeeCompetenceHeatmap,
            departmentId = 1,
            sortBy = "name"
        };

        var response = await _client.PostAsJsonAsync("/api/assessments/export", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        Assert.Equal(
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            response.Content.Headers.ContentType?.MediaType
        );
    }

    // [Fact]
    // public async Task Position_Matrix_Report_Should_Not_Fail()
    // {
    //     var request = new
    //     {
    //         reportType = ReportType.PositionMatrix,
    //         positionId = 1
    //     };
    //
    //     var response = await _client.PostAsJsonAsync("/api/assessments/export", request);
    //
    //     Assert.True(
    //         response.StatusCode == HttpStatusCode.OK ||
    //         response.StatusCode == HttpStatusCode.BadRequest
    //     );
    // }
}