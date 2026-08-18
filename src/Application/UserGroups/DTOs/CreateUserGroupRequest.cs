namespace Raga.Application.UserGroups.DTOs;

public sealed record CreateUserGroupRequest(string? Name, long OwnerId);
