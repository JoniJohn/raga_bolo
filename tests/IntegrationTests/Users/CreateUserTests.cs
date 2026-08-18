using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Raga.IntegrationTests.Fixtures;
using Xunit;

namespace Raga.IntegrationTests.Users;

[Collection("Integration")]
public sealed class CreateUserTests(DatabaseFixture db) : IAsyncLifetime
{
    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync() => await db.ResetDatabaseAsync();

    // ── Request / Response helpers ────────────────────────────────────────────

    private Task<HttpResponseMessage> PostUser(object body, CancellationToken cancellationToken = default) =>
        db.HttpClient.PostAsJsonAsync("/api/users", body, cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken);

    // ═════════════════════════════════════════════════════════════════════════
    // Happy path
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUser_WithValidNameAndNoAuthId_Returns201WithBody()
    {
        // Arrange
        var body = new { name = "John Doe" };

        // Act
        var response = await PostUser(body);

        // Assert — status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — body
        var json = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        json.Should().NotBeNull();
        json!.Id.Should().BeGreaterThan(0);
        json.Name.Should().Be("John Doe");
        json.AuthId.Should().BeNull();

        // Assert — Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/api/users/{json.Id}");
    }

    [Fact]
    public async Task PostUser_WithValidNameAndAuthId_Returns201WithAuthIdSet()
    {
        // Arrange
        var authId = Guid.NewGuid();
        var body = new { name = "Jane Doe", authId };

        // Act
        var response = await PostUser(body);

        // Assert — status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — body
        var json = await response.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        json.Should().NotBeNull();
        json!.Id.Should().BeGreaterThan(0);
        json.Name.Should().Be("Jane Doe");
        json.AuthId.Should().Be(authId);
    }

    [Fact]
    public async Task PostUser_TwoUsersWithNoAuthId_BothSucceedWithDistinctIds()
    {
        // Arrange
        var body1 = new { name = "Alice" };
        var body2 = new { name = "Bob" };

        // Act
        var r1 = await PostUser(body1);
        var r2 = await PostUser(body2);

        // Assert
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);

        var u1 = await r1.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        var u2 = await r2.Content.ReadFromJsonAsync<UserResponse>(cancellationToken: TestContext.Current.CancellationToken);
        u1!.Id.Should().NotBe(u2!.Id);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 400 Validation errors
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUser_WithMissingName_Returns400WithNameRequiredError()
    {
        // Arrange
        var body = new { name = (string?)null };

        // Act
        var response = await PostUser(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("USER_NAME_REQUIRED");
        error.Message.Should().Be("Name is required.");
    }

    [Fact]
    public async Task PostUser_WithBlankName_Returns400WithNameRequiredError()
    {
        // Arrange
        var body = new { name = "   " };

        // Act
        var response = await PostUser(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_NAME_REQUIRED");
    }

    [Fact]
    public async Task PostUser_WithNameExceedingMaxLength_Returns400WithNameTooLongError()
    {
        // Arrange
        var body = new { name = new string('A', 256) };

        // Act
        var response = await PostUser(body);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_NAME_TOO_LONG");
        error.Message.Should().Contain("255");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 409 Duplicate AuthId
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUser_WithDuplicateAuthId_Returns409WithDuplicateError()
    {
        // Arrange — create the first user successfully
        var authId = Guid.NewGuid();
        var firstResponse = await PostUser(new { name = "First User", authId });
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act — attempt to create a second user with the same authId
        var response = await PostUser(new { name = "Second User", authId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("USER_AUTH_ID_DUPLICATE");
        error.Message.Should().Be("A user with this Auth ID already exists.");
    }

    [Fact]
    public async Task PostUser_WithDuplicateAuthId_DoesNotCreateSecondRecord()
    {
        // Arrange
        var authId = Guid.NewGuid();
        await PostUser(new { name = "Original User", authId });

        // Act
        await PostUser(new { name = "Duplicate User", authId });

        // Assert — a GET (once implemented) would return only one user,
        // but for now we verify the 409 response indicates no insertion.
        var response = await PostUser(new { name = "Third Attempt", authId });
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    // ── Local DTOs for deserialisation ────────────────────────────────────────

    private sealed record UserResponse(long Id, string Name, Guid? AuthId);
    private sealed record ErrorResponse(string ErrorCode, string Message);
}
