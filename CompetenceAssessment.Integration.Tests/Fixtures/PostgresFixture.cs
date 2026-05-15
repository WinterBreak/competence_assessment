using Testcontainers.PostgreSql;
using Xunit;

public class PostgresFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; private set; }

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        Container = new PostgreSqlBuilder()
            .WithDatabase("competence_test")
            .WithUsername("test")
            .WithPassword("test")
            .Build();

        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}