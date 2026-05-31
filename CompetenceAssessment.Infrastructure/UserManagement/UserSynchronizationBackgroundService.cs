using CompetenceAssessment.Domain.UserManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CompetenceAssessment.Infrastructure.BackgroundServices;

public class UserSynchronizationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UserSynchronizationBackgroundService> _logger;
    private readonly UserSynchronizationOptions _options;

    public UserSynchronizationBackgroundService(
        IServiceProvider serviceProvider,
        IOptions<UserSynchronizationOptions> options,
        ILogger<UserSynchronizationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var synchronizationService = scope.ServiceProvider.GetRequiredService<IUserSynchronizationService>();
                
                var result = await synchronizationService.SynchronizeAsync(stoppingToken);
                
                _logger.LogInformation("Запланированная синхронизация завершена");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запланированной синхронизации пользователей");
            }
            
            await Task.Delay(TimeSpan.FromHours(_options.IntervalHours), stoppingToken);
        }
    }
}

public class UserSynchronizationOptions
{
    public double IntervalHours { get; set; } = 24;
}