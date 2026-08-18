namespace Raga.Application.Users.DTOs;

public sealed record CreateUserRequest(string? Name, Guid? AuthId);
