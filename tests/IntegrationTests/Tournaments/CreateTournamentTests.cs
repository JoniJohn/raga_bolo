using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Raga.IntegrationTests.Fixtures;
using Xunit;

namespace Raga.IntegrationTests.Tournaments;

[Collection("Integration")]
public sealed class CreateTournamentTests(DatabaseFixture db) : IAsyncLifetime
{
    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync() => await db.ResetDatabaseAsync();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Task<HttpResponseMessage> PostTournament(object body, CancellationToken cancellationToken = default) =>
        db.HttpClient.PostAsJsonAsync("/api/tournaments", body, cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken);

    /// <summary>Seeds a user and returns its generated Id.</summary>
    private async Task<long> CreateUserAsync(string name = "Tournament Owner", CancellationToken cancellationToken = default)
    {
        var ct = cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken;
        var response = await db.HttpClient.PostAsJsonAsync("/api/users", new { name }, ct);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<UserStub>(cancellationToken: ct);
        return user!.Id;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Happy path
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostTournament_WithValidData_Returns201WithBodyAndLocationHeader()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var body = new
        {
            name = "Summer Championship 2026",
            refNumber = "TOURN-2026-001",
            description = "Annual regional summer tournament",
            logoUrl = "https://example.com/logo.png",
            ownerId,
            tournamentCupTypeId = 1
        };

        // Act
        var response = await PostTournament(body);

        // Assert — status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — body
        var json = await response.Content.ReadFromJsonAsync<TournamentResponse>(cancellationToken: TestContext.Current.CancellationToken);
        json.Should().NotBeNull();
        json!.Id.Should().BeGreaterThan(0);
        json.Name.Should().Be("Summer Championship 2026");
        json.RefNumber.Should().Be("TOURN-2026-001");
        json.Description.Should().Be("Annual regional summer tournament");
        json.LogoUrl.Should().Be("https://example.com/logo.png");
        json.OwnerId.Should().Be(ownerId);
        json.TournamentCupTypeId.Should().Be(1);
        json.IsActive.Should().BeTrue();

        // Assert — Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/api/tournaments/{json.Id}");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 400 Validation errors
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostTournament_WithMissingName_Returns400WithNameRequiredError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var body = new
        {
            name = (string?)null,
            refNumber = "TOURN-01",
            ownerId,
            tournamentCupTypeId = 1
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("TOURNAMENT_NAME_REQUIRED");
        error.Message.Should().Be("Tournament name is required.");
    }

    [Fact]
    public async Task PostTournament_WithMissingRefNumber_Returns400WithRefNumberRequiredError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var body = new
        {
            name = "Valid Name",
            refNumber = (string?)null,
            ownerId,
            tournamentCupTypeId = 1
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("TOURNAMENT_REF_NUMBER_REQUIRED");
        error.Message.Should().Be("Reference number is required.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PostTournament_WithInvalidOwnerId_Returns400WithOwnerInvalidError(long ownerId)
    {
        // Arrange
        var body = new
        {
            name = "Valid Name",
            refNumber = "TOURN-01",
            ownerId,
            tournamentCupTypeId = 1
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("TOURNAMENT_OWNER_INVALID");
        error.Message.Should().Be("Owner ID must be a positive number.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PostTournament_WithInvalidCupTypeId_Returns400WithCupTypeIdInvalidError(int tournamentCupTypeId)
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var body = new
        {
            name = "Valid Name",
            refNumber = "TOURN-01",
            ownerId,
            tournamentCupTypeId
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("TOURNAMENT_CUP_TYPE_INVALID");
        error.Message.Should().Be("Tournament Cup Type ID must be a positive number.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 404 Owner or CupType not found
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostTournament_WhenOwnerDoesNotExist_Returns404WithOwnerNotFoundError()
    {
        // Arrange
        const long nonExistentOwnerId = 999_999;
        var body = new
        {
            name = "Orphan Tournament",
            refNumber = "TOURN-ORPHAN",
            ownerId = nonExistentOwnerId,
            tournamentCupTypeId = 1
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("USER_NOT_FOUND");
        error.Message.Should().Be($"User with ID {nonExistentOwnerId} was not found.");
    }

    [Fact]
    public async Task PostTournament_WhenCupTypeDoesNotExist_Returns404WithCupTypeNotFoundError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        const int nonExistentCupTypeId = 999;
        var body = new
        {
            name = "Invalid Cup Tournament",
            refNumber = "TOURN-CUP-INV",
            ownerId,
            tournamentCupTypeId = nonExistentCupTypeId
        };

        // Act
        var response = await PostTournament(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("TOURNAMENT_CUP_TYPE_NOT_FOUND");
        error.Message.Should().Be($"Tournament cup type with ID {nonExistentCupTypeId} was not found.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 409 Duplicate RefNumber
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostTournament_WithDuplicateRefNumber_Returns409WithRefNumberDuplicateError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var firstBody = new
        {
            name = "First Tournament",
            refNumber = "DUPLICATE-REF",
            ownerId,
            tournamentCupTypeId = 1
        };
        var firstResponse = await PostTournament(firstBody);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act
        var secondBody = new
        {
            name = "Second Tournament",
            refNumber = "DUPLICATE-REF",
            ownerId,
            tournamentCupTypeId = 1
        };
        var response = await PostTournament(secondBody);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("TOURNAMENT_REF_NUMBER_DUPLICATE");
        error.Message.Should().Be("A tournament with reference number 'DUPLICATE-REF' already exists.");
    }

    // ── Local DTOs for deserialisation ────────────────────────────────────────

    private sealed record UserStub(long Id);
    private sealed record TournamentResponse(
        long Id,
        string Name,
        string RefNumber,
        string? Description,
        string? LogoUrl,
        long OwnerId,
        int TournamentCupTypeId,
        bool IsActive,
        DateTimeOffset CreatedAt,
        DateTimeOffset? UpdatedAt);
    private sealed record ErrorResponse(string ErrorCode, string Message);
}
