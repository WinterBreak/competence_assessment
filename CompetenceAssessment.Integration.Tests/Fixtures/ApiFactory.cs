using CompetenceAssessment.Infrastructure;
using CompetenceAssessment.Infrastructure.UserManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public ApiFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ReplaceDbContext<AssessmentContext>(services);
            ReplaceDbContext<UserManagementContext>(services);

            services.AddDbContext<AssessmentContext>(options =>
                options.UseNpgsql(_connectionString));

            services.AddDbContext<UserManagementContext>(options =>
                options.UseNpgsql(_connectionString));
        });
    }

    private static void ReplaceDbContext<T>(IServiceCollection services)
        where T : DbContext
    {
        var descriptor = services.SingleOrDefault(
            d => d.ServiceType == typeof(DbContextOptions<T>));

        if (descriptor != null)
            services.Remove(descriptor);
    }
}