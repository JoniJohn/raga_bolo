using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Raga.IntegrationTests.Fixtures;
using Xunit;

namespace Raga.IntegrationTests.UserGroups;

[Collection("Integration")]
public sealed class AddUserGroupMemberTests(DatabaseFixture db) : IAsyncLifetime
{
    // ── Lifecycle ─────────────────────────────────────────────────────────────

    public ValueTask InitializeAsync() => ValueTask.CompletedTask;

    public async ValueTask DisposeAsync() => await db.ResetDatabaseAsync();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private Task<HttpResponseMessage> PostMember(long groupId, object body, CancellationToken cancellationToken = default) =>
        db.HttpClient.PostAsJsonAsync($"/api/user-groups/{groupId}/members", body, cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken);

    private async Task<long> CreateUserAsync(string name = "Test User", CancellationToken cancellationToken = default)
    {
        var ct = cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken;
        var response = await db.HttpClient.PostAsJsonAsync("/api/users", new { name }, ct);
        response.EnsureSuccessStatusCode();
        var user = await response.Content.ReadFromJsonAsync<IdStub>(cancellationToken: ct);
        return user!.Id;
    }

    private async Task<long> CreateUserGroupAsync(long ownerId, string name = "Test Group", CancellationToken cancellationToken = default)
    {
        var ct = cancellationToken == default ? TestContext.Current.CancellationToken : cancellationToken;
        var response = await db.HttpClient.PostAsJsonAsync("/api/user-groups", new { name, ownerId }, ct);
        response.EnsureSuccessStatusCode();
        var group = await response.Content.ReadFromJsonAsync<GroupIdStub>(cancellationToken: ct);
        return group!.Id;
    }

    // ═════════════════════════════════════════════════════════════════════════
    // Happy path
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostMember_WithValidGroupAndUser_Returns201WithBody()
    {
        // Arrange
        var ownerId = await CreateUserAsync("Owner");
        var userId = await CreateUserAsync("Member");
        var groupId = await CreateUserGroupAsync(ownerId);

        // Act
        var response = await PostMember(groupId, new { userId });

        // Assert — status
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert — body
        var json = await response.Content.ReadFromJsonAsync<MemberResponse>(cancellationToken: TestContext.Current.CancellationToken);
        json.Should().NotBeNull();
        json!.UserGroupId.Should().Be(groupId);
        json.UserId.Should().Be(userId);

        // Assert — Location header
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString()
            .Should().EndWith($"/api/user-groups/{groupId}/members/{userId}");
    }

    [Fact]
    public async Task PostMember_AddingMultipleMembersToSameGroup_AllSucceed()
    {
        // Arrange
        var ownerId = await CreateUserAsync("Owner");
        var userId1 = await CreateUserAsync("Member One");
        var userId2 = await CreateUserAsync("Member Two");
        var groupId = await CreateUserGroupAsync(ownerId);

        // Act
        var r1 = await PostMember(groupId, new { userId = userId1 });
        var r2 = await PostMember(groupId, new { userId = userId2 });

        // Assert
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task PostMember_SameUserAddedToDifferentGroups_BothSucceed()
    {
        // Arrange
        var ownerId = await CreateUserAsync("Owner");
        var userId = await CreateUserAsync("Shared Member");
        var groupId1 = await CreateUserGroupAsync(ownerId, "Group One");
        var groupId2 = await CreateUserGroupAsync(ownerId, "Group Two");

        // Act
        var r1 = await PostMember(groupId1, new { userId });
        var r2 = await PostMember(groupId2, new { userId });

        // Assert
        r1.StatusCode.Should().Be(HttpStatusCode.Created);
        r2.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 400 Validation errors
    // ═════════════════════════════════════════════════════════════════════════

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task PostMember_WithInvalidUserId_Returns400WithUserIdInvalidError(long userId)
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var groupId = await CreateUserGroupAsync(ownerId);

        // Act
        var response = await PostMember(groupId, new { userId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_MEMBER_USER_ID_INVALID");
        error.Message.Should().Be("User ID must be a positive number.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 404 Not found
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostMember_WhenGroupDoesNotExist_Returns404WithGroupNotFoundError()
    {
        // Arrange
        var userId = await CreateUserAsync();
        const long nonExistentGroupId = 999_999;

        // Act
        var response = await PostMember(nonExistentGroupId, new { userId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_NOT_FOUND");
        error.Message.Should().Be("User group does not exist.");
    }

    [Fact]
    public async Task PostMember_WhenUserDoesNotExist_Returns404WithUserNotFoundError()
    {
        // Arrange
        var ownerId = await CreateUserAsync();
        var groupId = await CreateUserGroupAsync(ownerId);
        const long nonExistentUserId = 999_999;

        // Act
        var response = await PostMember(groupId, new { userId = nonExistentUserId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_NOT_FOUND");
        error.Message.Should().Be("User does not exist.");
    }

    // ═════════════════════════════════════════════════════════════════════════
    // 409 Duplicate member
    // ═════════════════════════════════════════════════════════════════════════

    [Fact]
    public async Task PostMember_WhenUserAlreadyMember_Returns409WithDuplicateError()
    {
        // Arrange — add the member once successfully
        var ownerId = await CreateUserAsync("Owner");
        var userId = await CreateUserAsync("Member");
        var groupId = await CreateUserGroupAsync(ownerId);

        var firstAdd = await PostMember(groupId, new { userId });
        firstAdd.StatusCode.Should().Be(HttpStatusCode.Created);

        // Act — attempt to add same user again
        var response = await PostMember(groupId, new { userId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_MEMBER_DUPLICATE");
        error.Message.Should().Be("User is already a member of this group.");
    }

    [Fact]
    public async Task PostMember_WhenOwnerAddedAsExplicitMember_Returns409WithDuplicateError()
    {
        // Arrange — owner is implicitly a member on group creation
        var ownerId = await CreateUserAsync("Owner");
        var groupId = await CreateUserGroupAsync(ownerId);

        // Act — try to add the owner explicitly
        var response = await PostMember(groupId, new { userId = ownerId });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: TestContext.Current.CancellationToken);
        error!.ErrorCode.Should().Be("USER_GROUP_MEMBER_DUPLICATE");
        error.Message.Should().Be("User is already a member of this group.");
    }

    // ── Local DTOs for deserialisation ────────────────────────────────────────

    private sealed record IdStub(long Id);
    private sealed record GroupIdStub(long Id);
    private sealed record MemberResponse(long UserGroupId, long UserId);
    private sealed record ErrorResponse(string ErrorCode, string Message);
}
