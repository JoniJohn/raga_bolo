using Raga.Application.Users.DTOs;
using Raga.Domain.Common;

namespace Raga.Application.Users;

public interface IUserService
{
    Task<Result<UserResponse>> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
