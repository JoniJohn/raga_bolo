using Microsoft.AspNetCore.Mvc;
using Raga.Application.Users;
using Raga.Application.Users.DTOs;

namespace Raga.Api.Controllers;

[ApiController]
[Route("api/users")]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>Onboard a new user profile.</summary>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Validation failure (name required or too long).</response>
    /// <response code="409">A user with the supplied AuthId already exists.</response>
    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = new ErrorResponse(result.ErrorCode, result.ErrorMessage);

            return result.ErrorCode == "USER_AUTH_ID_DUPLICATE"
                ? Conflict(error)
                : BadRequest(error);
        }

        return Created($"/api/users/{result.Value!.Id}", result.Value);
    }
}

/// <summary>Standard error envelope returned on 4xx responses.</summary>
public sealed record ErrorResponse(string ErrorCode, string Message);
