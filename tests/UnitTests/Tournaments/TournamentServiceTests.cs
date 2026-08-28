using FluentAssertions;
using NSubstitute;
using Raga.Application.Tournaments;
using Raga.Application.Tournaments.DTOs;
using Raga.Application.Users;
using Raga.Domain.Tournaments;
using Xunit;

namespace Raga.UnitTests.Tournaments;

public sealed class TournamentServiceTests
{
    private readonly ITournamentRepository _tournamentRepository = Substitute.For<ITournamentRepository>();
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly TournamentService _sut;

    public TournamentServiceTests()
    {
        _sut = new TournamentService(_tournamentRepository, _userRepository);
    }

    [Fact]
    public async Task CreateAsync_WhenDomainValidationFails_ReturnsFailureWithoutCallingRepositories()
    {
        // Arrange
        var request = new CreateTournamentRequest(null, "TOURN-01", null, null, 1, 1);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_NAME_REQUIRED");

        await _userRepository.DidNotReceiveWithAnyArgs().ExistsAsync(default);
        await _tournamentRepository.DidNotReceiveWithAnyArgs().TournamentCupTypeExistsAsync(default);
        await _tournamentRepository.DidNotReceiveWithAnyArgs().RefNumberExistsAsync(default!);
    }

    [Fact]
    public async Task CreateAsync_WhenOwnerDoesNotExist_ReturnsUserNotFoundFailure()
    {
        // Arrange
        var request = new CreateTournamentRequest("Summer Cup", "TOURN-01", null, null, 999, 1);
        _userRepository.ExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("USER_NOT_FOUND");
        result.ErrorMessage.Should().Be("User with ID 999 was not found.");
    }

    [Fact]
    public async Task CreateAsync_WhenCupTypeDoesNotExist_ReturnsCupTypeNotFoundFailure()
    {
        // Arrange
        var request = new CreateTournamentRequest("Summer Cup", "TOURN-01", null, null, 1, 999);
        _userRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _tournamentRepository.TournamentCupTypeExistsAsync(999, Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_CUP_TYPE_NOT_FOUND");
        result.ErrorMessage.Should().Be("Tournament cup type with ID 999 was not found.");
    }

    [Fact]
    public async Task CreateAsync_WhenRefNumberAlreadyExists_ReturnsRefNumberDuplicateFailure()
    {
        // Arrange
        var request = new CreateTournamentRequest("Summer Cup", "EXISTING-REF", null, null, 1, 1);
        _userRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _tournamentRepository.TournamentCupTypeExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _tournamentRepository.RefNumberExistsAsync("EXISTING-REF", Arg.Any<CancellationToken>()).Returns(true);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("TOURNAMENT_REF_NUMBER_DUPLICATE");
        result.ErrorMessage.Should().Be("A tournament with reference number 'EXISTING-REF' already exists.");
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_PersistsAndReturnsSuccessResponse()
    {
        // Arrange
        var request = new CreateTournamentRequest("Summer Cup", "TOURN-01", "Desc", "https://logo.png", 1, 1);
        _userRepository.ExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _tournamentRepository.TournamentCupTypeExistsAsync(1, Arg.Any<CancellationToken>()).Returns(true);
        _tournamentRepository.RefNumberExistsAsync("TOURN-01", Arg.Any<CancellationToken>()).Returns(false);

        // Act
        var result = await _sut.CreateAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Summer Cup");
        result.Value.RefNumber.Should().Be("TOURN-01");
        result.Value.Description.Should().Be("Desc");
        result.Value.LogoUrl.Should().Be("https://logo.png");
        result.Value.OwnerId.Should().Be(1);
        result.Value.TournamentCupTypeId.Should().Be(1);

        await _tournamentRepository.Received(1).AddAsync(Arg.Any<Tournament>(), Arg.Any<CancellationToken>());
        await _tournamentRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
