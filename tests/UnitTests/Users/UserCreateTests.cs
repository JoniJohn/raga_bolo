using FluentAssertions;
using Raga.Domain.Users;
using Xunit;

namespace Raga.UnitTests.Users;

public sealed class UserCreateTests
{
    // ── Success cases ─────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidNameAndNoAuthId_ReturnsSuccessWithCorrectData()
    {
        // Arrange
        const string name = "John Doe";

        // Act
        var result = User.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be(name);
        result.Value.AuthId.Should().BeNull();
    }

    [Fact]
    public void Create_WithValidNameAndAuthId_ReturnsSuccessWithAuthIdSet()
    {
        // Arrange
        const string name = "Jane Doe";
        var authId = Guid.NewGuid();

        // Act
        var result = User.Create(name, authId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be(name);
        result.Value.AuthId.Should().Be(authId);
    }

    [Fact]
    public void Create_WithNameExactlyAtMaxLength_ReturnsSuccess()
    {
        // Arrange
        var name = new string('A', 255);

        // Act
        var result = User.Create(name);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    // ── Name required failures ────────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameIsNullOrWhitespace_ReturnsNameRequiredFailure(string? name)
    {
        // Arrange & Act
        var result = User.Create(name!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_NAME_REQUIRED");
        result.ErrorMessage.Should().Be("Name is required.");
    }

    // ── Name max length failures ──────────────────────────────────────────────

    [Fact]
    public void Create_WhenNameExceedsMaxLength_ReturnsNameTooLongFailure()
    {
        // Arrange
        var name = new string('A', 256);

        // Act
        var result = User.Create(name);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_NAME_TOO_LONG");
        result.ErrorMessage.Should().Contain("255");
    }

    // ── Rule precedence ───────────────────────────────────────────────────────

    [Fact]
    public void Create_WhenNameIsWhitespace_ReturnsRequiredErrorNotMaxLengthError()
    {
        // Arrange — whitespace is both "empty" and could technically be long,
        // but required rule fires first.
        var name = new string(' ', 300);

        // Act
        var result = User.Create(name);

        // Assert
        result.ErrorCode.Should().Be("USER_NAME_REQUIRED");
    }

    // ── Id is not set by factory ──────────────────────────────────────────────

    [Fact]
    public void Create_IdIsDefaultLong_UntilPersistedByDatabase()
    {
        // Arrange & Act
        var result = User.Create("Test User");

        // Assert — Id is assigned by the DB identity column, not the factory
        result.Value!.Id.Should().Be(0);
    }
}
