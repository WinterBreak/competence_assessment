using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CompetenceAssessment.Infrastructure.UserManagement;

public class UserManagementContextFactory : IDesignTimeDbContextFactory<UserManagementContext>
{
    public UserManagementContext CreateDbContext(string[] args)
    {
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../CompetenceAssessment.Web");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .Build();
        
        var optionsBuilder = new DbContextOptionsBuilder<UserManagementContext>();
        
        var connectionString = configuration.GetConnectionString("CompetenceAssessment");
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions => 
        {
            npgsqlOptions.MigrationsAssembly("CompetenceAssessment.Infrastructure");
        });
        
        return new UserManagementContext(optionsBuilder.Options);
    }
}