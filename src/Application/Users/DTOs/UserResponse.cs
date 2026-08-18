namespace Raga.Application.Users.DTOs;

public sealed record UserResponse(long Id, string Name, Guid? AuthId);
