using Microsoft.AspNetCore.Mvc;
using Raga.Application.UserGroups;
using Raga.Application.UserGroups.DTOs;

namespace Raga.Api.Controllers;

[ApiController]
[Route("api/user-groups/{id:long}/members")]
public sealed class UserGroupMembersController(IUserGroupMemberService memberService) : ControllerBase
{
    /// <summary>Add a user to a user group.</summary>
    /// <param name="id">The user group ID.</param>
    /// <response code="201">Member added successfully.</response>
    /// <response code="400">userId is not a positive number.</response>
    /// <response code="404">User group or user does not exist.</response>
    /// <response code="409">User is already a member of this group.</response>
    [HttpPost]
    [ProducesResponseType<UserGroupMemberResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddMember(
        long id,
        [FromBody] AddMemberRequest request,
        CancellationToken cancellationToken)
    {
        var result = await memberService.AddMemberAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            var error = new ErrorResponse(result.ErrorCode, result.ErrorMessage);

            return result.ErrorCode switch
            {
                "USER_GROUP_NOT_FOUND" or "USER_NOT_FOUND" => NotFound(error),
                "USER_GROUP_MEMBER_DUPLICATE"              => Conflict(error),
                _                                          => BadRequest(error)
            };
        }

        return Created(
            $"/api/user-groups/{id}/members/{result.Value!.UserId}",
            result.Value);
    }
}
