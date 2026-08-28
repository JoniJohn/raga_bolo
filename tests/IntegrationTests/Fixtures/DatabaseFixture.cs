using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Raga.Infrastructure.Persistence;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;

namespace Raga.IntegrationTests.Fixtures;

/// <summary>
/// Spins up a SINGLE PostgreSQL Testcontainer for the entire integration test suite.
/// Shared via xUnit ICollectionFixture — only one container is ever created per test run.
/// </summary>
public sealed class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:16-alpine")
        .WithDatabase("raga_test")
        .WithUsername("raga")
        .WithPassword("raga_secret")
        .Build();

    private Respawner _respawner = null!;
    private WebApplicationFactory<Program> _factory = null!;

    public HttpClient HttpClient { get; private set; } = null!;
    public IServiceScope Scope { get; private set; } = null!;

    public async ValueTask InitializeAsync()
    {
        await _postgres.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(host =>
            {
                host.ConfigureServices(services =>
                {
                    // Replace the registered DbContext with one pointing at the Testcontainer.
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                    if (descriptor is not null)
                        services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(_postgres.GetConnectionString()));
                });
            });

        HttpClient = _factory.CreateClient();
        Scope = _factory.Services.CreateScope();

        // Apply all pending EF Core migrations.
        var db = Scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        // Initialise Respawn for fast state reset between tests.
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();
        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = ["public", "usr", "tour"]
        });
    }

    /// <summary>
    /// Resets the database to a clean state. Call this from each test or test class.
    /// </summary>
    public async Task ResetDatabaseAsync()
    {
        await using var conn = new NpgsqlConnection(_postgres.GetConnectionString());
        await conn.OpenAsync();
        await _respawner.ResetAsync(conn);
    }

    public async ValueTask DisposeAsync()
    {
        Scope?.Dispose();
        if (_factory is not null) await _factory.DisposeAsync();
        if (_postgres is not null) await _postgres.DisposeAsync();
    }
}
