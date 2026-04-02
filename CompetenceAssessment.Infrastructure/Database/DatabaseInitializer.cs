using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompetenceAssessment.Infrastructure.Database;

public interface IDatabaseInitializer
    {
        void Migrate();
        void Rollback(int steps = 1);
        void MigrateToVersion(long version);
    }

    public class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseInitializer> _logger;

        public DatabaseInitializer(IServiceProvider serviceProvider, ILogger<DatabaseInitializer> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public void Migrate()
        {
            _logger.LogInformation("Starting database migrations...");
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                
                try
                {
                    runner.MigrateUp();
                    _logger.LogInformation("Database migrations completed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while running migrations.");
                    throw;
                }
            }
        }

        public void Rollback(int steps = 1)
        {
            _logger.LogInformation($"Rolling back {steps} migration(s)...");
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                
                try
                {
                    runner.Rollback(steps);
                    _logger.LogInformation($"Successfully rolled back {steps} migration(s).");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while rolling back migrations.");
                    throw;
                }
            }
        }

        public void MigrateToVersion(long version)
        {
            _logger.LogInformation($"Migrating to version {version}...");
            
            using (var scope = _serviceProvider.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                
                try
                {
                    runner.MigrateUp(version);
                    _logger.LogInformation($"Successfully migrated to version {version}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"An error occurred while migrating to version {version}.");
                    throw;
                }
            }
        }
    }