using FluentAssertions;
using Raga.Domain.UserGroups;
using Xunit;

namespace Raga.UnitTests.UserGroups;

public sealed class UserGroupCreateTests
{
    // ── Success cases ─────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidNameAndOwnerId_ReturnsSuccess()
    {
        // Arrange & Act
        var result = UserGroup.Create("Tournament Admins", 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Tournament Admins");
        result.Value.OwnerId.Should().Be(1);
    }

    [Fact]
    public void Create_WithNameExactlyAtMaxLength_ReturnsSuccess()
    {
        var result = UserGroup.Create(new string('A', 200), 1);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_IdIsDefaultLong_UntilPersistedByDatabase()
    {
        var result = UserGroup.Create("Admins", 1);
        result.Value!.Id.Should().Be(0);
    }

    // ── Name required failures ────────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameIsNullOrWhitespace_ReturnsNameRequiredFailure(string? name)
    {
        var result = UserGroup.Create(name!, 1);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_GROUP_NAME_REQUIRED");
        result.ErrorMessage.Should().Be("Name is required.");
    }

    // ── Name max length failures ──────────────────────────────────────────────

    [Fact]
    public void Create_WhenNameExceedsMaxLength_ReturnsNameTooLongFailure()
    {
        var result = UserGroup.Create(new string('A', 201), 1);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_GROUP_NAME_TOO_LONG");
        result.ErrorMessage.Should().Contain("200");
    }

    // ── OwnerId invalid failures ──────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void Create_WhenOwnerIdIsNotPositive_ReturnsOwnerInvalidFailure(long ownerId)
    {
        var result = UserGroup.Create("Admins", ownerId);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_GROUP_OWNER_INVALID");
        result.ErrorMessage.Should().Be("Owner ID must be a positive number.");
    }

    // ── Rule precedence ───────────────────────────────────────────────────────

    [Fact]
    public void Create_WhenNameIsBlankAndOwnerIdInvalid_ReturnsNameRequiredErrorFirst()
    {
        // Name required fires before ownerId valid
        var result = UserGroup.Create("   ", 0);

        result.ErrorCode.Should().Be("USER_GROUP_NAME_REQUIRED");
    }

    [Fact]
    public void Create_WhenNameIsTooLongAndOwnerIdInvalid_ReturnsNameTooLongErrorFirst()
    {
        // MaxLength fires before ownerId valid
        var result = UserGroup.Create(new string('A', 201), 0);

        result.ErrorCode.Should().Be("USER_GROUP_NAME_TOO_LONG");
    }
}
