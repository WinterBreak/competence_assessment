using CompetenceAssessment.Infrastructure.UserManagement;
using CompetenceAssessment.IntegrationTests.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace CompetenceAssessment.IntegrationTests;

public class IntegrationTestFixture : WebApplicationFactory<Program>
{
    public HttpClient Client { get; }

    public IntegrationTestFixture()
    {
        Client = CreateClient();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    "Test", _ => { });

            services.PostConfigureAll<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "Test";
                options.DefaultChallengeScheme = "Test";
            });
        });
        
        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<UserManagementContext>();

            db.Database.EnsureCreated();
            
            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { Id = 1, Email = "test1@test.com" },
                    new User { Id = 2, Email = "test2@test.com" }
                );

                db.SaveChanges();
            }
        });
    }
}