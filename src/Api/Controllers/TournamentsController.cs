using Microsoft.AspNetCore.Mvc;
using Raga.Application.Tournaments;
using Raga.Application.Tournaments.DTOs;

namespace Raga.Api.Controllers;

[ApiController]
[Route("api/tournaments")]
public sealed class TournamentsController(ITournamentService tournamentService) : ControllerBase
{
    /// <summary>Register a new tournament.</summary>
    /// <response code="201">Tournament created successfully.</response>
    /// <response code="400">Validation failure (missing/invalid fields).</response>
    /// <response code="404">Owner user or cup type not found.</response>
    /// <response code="409">Reference number already exists.</response>
    [HttpPost]
    [ProducesResponseType<TournamentResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTournamentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await tournamentService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = new ErrorResponse(result.ErrorCode, result.ErrorMessage);

            return result.ErrorCode switch
            {
                "USER_NOT_FOUND" or "TOURNAMENT_CUP_TYPE_NOT_FOUND" => NotFound(error),
                "TOURNAMENT_REF_NUMBER_DUPLICATE" => Conflict(error),
                _ => BadRequest(error)
            };
        }

        return Created($"/api/tournaments/{result.Value!.Id}", result.Value);
    }
}
