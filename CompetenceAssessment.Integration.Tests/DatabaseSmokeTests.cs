using System.Net;
using Xunit;

namespace CompetenceAssessment.IntegrationTests.Infrastructure;

public class DatabaseSmokeTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;

    public DatabaseSmokeTests(IntegrationTestFixture fixture)
    {
        _client = fixture.Client;
    }

    [Fact]
    public async Task Api_Should_Start_And_Respond()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.True(
            response.StatusCode == HttpStatusCode.OK ||
            response.StatusCode == HttpStatusCode.NotFound
        );
    }
}