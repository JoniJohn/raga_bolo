using FluentAssertions;
using Raga.Domain.UserGroups;
using Xunit;

namespace Raga.UnitTests.UserGroups;

public sealed class UserGroupMemberCreateTests
{
    [Fact]
    public void Create_WithValidGroupAndUserId_ReturnsSuccess()
    {
        // Arrange & Act
        var result = UserGroupMember.Create(1, 42);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.UserGroupId.Should().Be(1);
        result.Value.UserId.Should().Be(42);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WhenUserIdIsNotPositive_ReturnsUserIdInvalidFailure(long userId)
    {
        // Arrange & Act
        var result = UserGroupMember.Create(1, userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_GROUP_MEMBER_USER_ID_INVALID");
        result.ErrorMessage.Should().Be("User ID must be a positive number.");
    }
}
