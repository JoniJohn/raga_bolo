using Microsoft.EntityFrameworkCore;
using Raga.Application.UserGroups;
using Raga.Application.Users;
using Raga.Infrastructure.Persistence;
using Raga.Infrastructure.Persistence.Repositories;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Infrastructure ────────────────────────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserGroupRepository, UserGroupRepository>();
builder.Services.AddScoped<IUserGroupService, UserGroupService>();
builder.Services.AddScoped<IUserGroupMemberRepository, UserGroupMemberRepository>();
builder.Services.AddScoped<IUserGroupMemberService, UserGroupMemberService>();

// ── API / MVC ─────────────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── OpenAPI ───────────────────────────────────────────────────────────────────
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info.Title = "Raga Bolo Tournament API";
        doc.Info.Version = "v1";
        doc.Info.Description = "Tournament management system: creation, participants, fixtures, and live match events.";

        // Health check endpoints are not picked up automatically — add manually.
        // OpenAPI.NET v2 uses HttpMethod as the operation key (no OperationType enum).
        doc.Paths ??= new Microsoft.OpenApi.OpenApiPaths();
        var healthOperation = new Microsoft.OpenApi.OpenApiOperation
        {
            Summary = "Health check",
            Description = "Returns the health status of the API and its dependencies (PostgreSQL).",
            Tags = new HashSet<Microsoft.OpenApi.OpenApiTagReference>
            {
                new Microsoft.OpenApi.OpenApiTagReference("Health", doc)
            },
            Responses = new Microsoft.OpenApi.OpenApiResponses
            {
                ["200"] = new Microsoft.OpenApi.OpenApiResponse { Description = "Healthy" },
                ["503"] = new Microsoft.OpenApi.OpenApiResponse { Description = "Unhealthy" }
            }
        };

        var healthPathItem = new Microsoft.OpenApi.OpenApiPathItem();
        healthPathItem.Operations ??= new Dictionary<System.Net.Http.HttpMethod, Microsoft.OpenApi.OpenApiOperation>();
        healthPathItem.Operations[System.Net.Http.HttpMethod.Get] = healthOperation;
        doc.Paths["/health"] = healthPathItem;

        return Task.CompletedTask;
    });
});

// ── Health Checks ─────────────────────────────────────────────────────────────
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("Default")!,
        name: "postgres",
        tags: ["db", "ready"]);

var app = builder.Build();

// ── OpenAPI / Scalar UI ───────────────────────────────────────────────────────
app.MapOpenApi();                      // serves /openapi/v1.json
app.MapScalarApiReference(options =>
{
    options.Title = "Raga Bolo API";
    options.Theme = ScalarTheme.DeepSpace;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
});                                    // serves /scalar/v1

// ── Health Check Endpoint ─────────────────────────────────────────────────────
app.MapHealthChecks("/health");

// ── Middleware ────────────────────────────────────────────────────────────────
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed for WebApplicationFactory in integration tests
public partial class Program { }
