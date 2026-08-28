using FluentAssertions;
using Raga.Domain.Tournaments;
using Xunit;

namespace Raga.UnitTests.Tournaments;

public sealed class TournamentCreateTests
{
    // ── Success cases ─────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidData_ReturnsSuccessWithCorrectData()
    {
        // Arrange
        const string name = "Summer Championship 2026";
        const string refNumber = "TOURN-2026-001";
        const string description = "Annual regional tournament";
        const string logoUrl = "https://example.com/logo.png";
        const long ownerId = 1;
        const int tournamentCupTypeId = 1;

        // Act
        var result = Tournament.Create(name, refNumber, description, logoUrl, ownerId, tournamentCupTypeId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be(name);
        result.Value.RefNumber.Should().Be(refNumber);
        result.Value.Description.Should().Be(description);
        result.Value.LogoUrl.Should().Be(logoUrl);
        result.Value.OwnerId.Should().Be(ownerId);
        result.Value.TournamentCupTypeId.Should().Be(tournamentCupTypeId);
    }

    [Fact]
    public void Create_WithOptionalFieldsNull_ReturnsSuccess()
    {
        // Arrange & Act
        var result = Tournament.Create("Premier League", "PL-2026", null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Description.Should().BeNull();
        result.Value.LogoUrl.Should().BeNull();
    }

    [Fact]
    public void Create_WithNameAndRefNumberExactlyAtMaxLength_ReturnsSuccess()
    {
        // Arrange
        var name = new string('A', 255);
        var refNumber = new string('B', 100);

        // Act
        var result = Tournament.Create(name, refNumber, null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_IdIsDefaultLong_UntilPersistedByDatabase()
    {
        // Arrange & Act
        var result = Tournament.Create("Test Tournament", "TEST-01", null, null, 1, 1);

        // Assert
        result.Value!.Id.Should().Be(0);
    }

    // ── Name validation failures ──────────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenNameIsNullOrWhitespace_ReturnsNameRequiredFailure(string? name)
    {
        // Arrange & Act
        var result = Tournament.Create(name!, "TOURN-01", null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_NAME_REQUIRED");
        result.ErrorMessage.Should().Be("Tournament name is required.");
    }

    [Fact]
    public void Create_WhenNameExceedsMaxLength_ReturnsNameTooLongFailure()
    {
        // Arrange
        var name = new string('A', 256);

        // Act
        var result = Tournament.Create(name, "TOURN-01", null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_NAME_TOO_LONG");
        result.ErrorMessage.Should().Contain("255");
    }

    // ── RefNumber validation failures ─────────────────────────────────────────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WhenRefNumberIsNullOrWhitespace_ReturnsRefNumberRequiredFailure(string? refNumber)
    {
        // Arrange & Act
        var result = Tournament.Create("Valid Name", refNumber!, null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_REF_NUMBER_REQUIRED");
        result.ErrorMessage.Should().Be("Reference number is required.");
    }

    [Fact]
    public void Create_WhenRefNumberExceedsMaxLength_ReturnsRefNumberTooLongFailure()
    {
        // Arrange
        var refNumber = new string('B', 101);

        // Act
        var result = Tournament.Create("Valid Name", refNumber, null, null, 1, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_REF_NUMBER_TOO_LONG");
        result.ErrorMessage.Should().Contain("100");
    }

    // ── OwnerId validation failures ───────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void Create_WhenOwnerIdIsNotPositive_ReturnsOwnerInvalidFailure(long ownerId)
    {
        // Arrange & Act
        var result = Tournament.Create("Valid Name", "TOURN-01", null, null, ownerId, 1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_OWNER_INVALID");
        result.ErrorMessage.Should().Be("Owner ID must be a positive number.");
    }

    // ── TournamentCupTypeId validation failures ───────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public void Create_WhenTournamentCupTypeIdIsNotPositive_ReturnsCupTypeIdInvalidFailure(int cupTypeId)
    {
        // Arrange & Act
        var result = Tournament.Create("Valid Name", "TOURN-01", null, null, 1, cupTypeId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_CUP_TYPE_INVALID");
        result.ErrorMessage.Should().Be("Tournament Cup Type ID must be a positive number.");
    }
}
