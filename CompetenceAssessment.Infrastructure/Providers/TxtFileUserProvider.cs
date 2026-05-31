using System.Text;
using CompetenceAssessment.Domain.UserManagement;
using Microsoft.Extensions.Options;

namespace CompetenceAssessment.Infrastructure.Providers;

public class TxtFileUserProvider : IExternalUserProvider
{
    private readonly TxtFileProviderOptions _options;

    public TxtFileUserProvider(IOptions<TxtFileProviderOptions> options)
    {
        _options = options.Value;
    }

    public async Task<List<ExternalUser>> GetUsersAsync(CancellationToken token = default)
    {
        if (!File.Exists(_options.FilePath))
            throw new FileNotFoundException($"Файл не найден: {_options.FilePath}");

        var lines = await File.ReadAllLinesAsync(_options.FilePath, Encoding.UTF8, token);
        var users = new List<ExternalUser>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            
            var parts = line.Split(';');
            if (parts.Length < 6) continue;

            var user = new ExternalUser
            {
                LastName = parts[0].Trim(),
                FirstName = parts[1].Trim(),
                SecondName = string.IsNullOrWhiteSpace(parts[2].Trim()) ? null : parts[2].Trim(),
                Email = parts[3].Trim(),
                PositionName = parts[4].Trim(),
                DepartmentName = parts[5].Trim(),
                BossEmail = parts.Length > 6 ? parts[6].Trim() : null
            };
            
            users.Add(user);
        }

        return users;
    }
}

public class TxtFileProviderOptions
{
    public string FilePath { get; set; } = "users.txt";
}