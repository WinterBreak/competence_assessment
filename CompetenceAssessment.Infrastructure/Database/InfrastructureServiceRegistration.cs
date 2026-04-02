using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CompetenceAssessment.Infrastructure.Database;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CompetenceAssessment");
        
        services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(InfrastructureServiceRegistration).Assembly)
                .For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole());
        
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        
        return services;
    }
    
    public static void RunMigrations(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            initializer.Migrate();
        }
    }
}