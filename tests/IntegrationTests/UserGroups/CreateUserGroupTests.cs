using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Raga.IntegrationTests.Fixtures;
using Xunit;

namespace Raga.IntegrationTests.UserGroups;

[Collection("Integration")]
public sealed class CreateUserGroupTests(DatabaseFixture db) : IAsyncLifetime
{
    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync() => await db.ResetDatabaseAsync();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Task<HttpResponseMessage> PostUserGroup(object body, CancellationToken cancellationToken = default) =>
        db.HttpClient.PostAsJsonAsync("/api/user-groups", body, cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken);

    /// <summary>Seeds a user and returns its generated Id.</summary>
    private async Task<long> CreateUserAsync(string name = "Owner User", CancellationToken cancellationToken = default)
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
    public async Task PostUserGroup_WithValidNameAndExistingOwner_Returns201WithBody()
    {
        // Arrange
        var ownerId = await CreateUserAsync();

        // Act
        var response = await PostUserGroup(new { name = "Tournament Admins", ownerId });

        // Assert — status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — body
        var json = await response.Content.ReadFromJsonAsync<UserGroupResponse>(cancellationToken: TestContext.Current.CancellationToken);
        json.Should().NotBeNull();
        json!.Id.Should().BeGreaterThan(0);
        json.Name.Should().Be("Tournament Admins");
        json.OwnerId.Should().Be(ownerId);

        // Assert — Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().EndWith($"/api/user-groups/{json.Id}");
    }

    [Fact]
    public async Task PostUserGroup_TwoGroupsWithSameOwner_BothSucceedWithDistinctIds()
    {
        // Arrange
        var ownerId = await CreateUserAsync();

        // Act
        var r1 = await PostUserGroup(new { name = "Group Alpha", ownerId });
        var r2 = await PostUserGroup(new { name = "Group Beta", ownerId });

        // Assert
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);

        var g1 = await r1.Content.ReadFromJsonAsync<UserGroupResponse>(cancellationToken: TestContext.Current.CancellationToken);
        var g2 = await r2.Content.ReadFromJsonAsync<UserGroupResponse>(cancellationToken: TestContext.Current.CancellationToken);
        g1!.Id.Should().NotBe(g2!.Id);
    }

    [Fact]
    public async Task PostUserGroup_TwoGroupsWithDifferentOwners_BothSucceed()
    {
        // Arrange
        var ownerId1 = await CreateUserAsync("Owner One");
        var ownerId2 = await CreateUserAsync("Owner Two");

        // Act
        var r1 = await PostUserGroup(new { name = "Group One", ownerId = ownerId1 });
        var r2 = await PostUserGroup(new { name = "Group Two", ownerId = ownerId2 });

        // Assert
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 400 Validation errors
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUserGroup_WithMissingName_Returns400WithNameRequiredError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();

        // Act
        var response = await PostUserGroup(new { name = (string?)null, ownerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_NAME_REQUIRED");
        error.Message.Should().Be("Name is required.");
    }

    [Fact]
    public async Task PostUserGroup_WithBlankName_Returns400WithNameRequiredError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();

        // Act
        var response = await PostUserGroup(new { name = "   ", ownerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_NAME_REQUIRED");
    }

    [Fact]
    public async Task PostUserGroup_WithNameExceedingMaxLength_Returns400WithNameTooLongError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();

        // Act
        var response = await PostUserGroup(new { name = new string('A', 201), ownerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_NAME_TOO_LONG");
        error.Message.Should().Contain("200");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PostUserGroup_WithInvalidOwnerId_Returns400WithOwnerInvalidError(long ownerId)
    {
        // Act
        var response = await PostUserGroup(new { name = "Admins", ownerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_OWNER_INVALID");
        error.Message.Should().Be("Owner ID must be a positive number.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 404 Owner not found
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostUserGroup_WhenOwnerDoesNotExist_Returns404WithOwnerNotFoundError()
    {
        // Arrange — use an ownerId that has never been inserted
        const long nonExistentOwnerId = 999_999;

        // Act
        var response = await PostUserGroup(new { name = "Orphan Group", ownerId = nonExistentOwnerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error.Should().NotBeNull();
        error!.ErrorCode.Should().Be("USER_GROUP_OWNER_NOT_FOUND");
        error.Message.Should().Be("Owner user does not exist.");
    }

    // ── Local DTOs for deserialisation ────────────────────────────────────────

    private sealed record UserStub(long Id);
    private sealed record UserGroupResponse(long Id, string Name, long OwnerId);
    private sealed record ErrorResponse(string ErrorCode, string Message);
}
