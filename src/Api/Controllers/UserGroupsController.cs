using Microsoft.AspNetCore.Mvc;
using Raga.Application.UserGroups;
using Raga.Application.UserGroups.DTOs;
using Raga.Api.Controllers;

namespace Raga.Api.Controllers;

[ApiController]
[Route("api/user-groups")]
public sealed class UserGroupsController(IUserGroupService userGroupService) : ControllerBase
{
    /// <summary>Register a new user group.</summary>
    /// <response code="201">User group created successfully.</response>
    /// <response code="400">Validation failure (name or ownerId invalid).</response>
    /// <response code="404">Owner user does not exist.</response>
    [HttpPost]
    [ProducesResponseType<UserGroupResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserGroupRequest request,
        CancellationToken cancellationToken)
    {
        var result = await userGroupService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = new ErrorResponse(result.ErrorCode, result.ErrorMessage);

            return result.ErrorCode == "USER_GROUP_OWNER_NOT_FOUND"
                ? NotFound(error)
                : BadRequest(error);
        }

        return Created($"/api/user-groups/{result.Value!.Id}", result.Value);
    }
}
