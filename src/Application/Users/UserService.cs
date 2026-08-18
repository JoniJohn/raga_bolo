using Raga.Application.Users.DTOs;
using Raga.Domain.Common;
using Raga.Domain.Users;

namespace Raga.Application.Users;

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<Result<UserResponse>> CreateAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        // 1. Cross-entity policy: duplicate AuthId check
        if (request.AuthId.HasValue)
        {
            var exists = await userRepository.AuthIdExistsAsync(request.AuthId.Value, cancellationToken);
            if (exists)
                return Result<UserResponse>.Failure(
                    "USER_AUTH_ID_DUPLICATE",
                    "A user with this Auth ID already exists.");
        }

        // 2. Domain validation via entity factory
        var createResult = User.Create(request.Name!, request.AuthId);
        if (!createResult.IsSuccess)
            return Result<UserResponse>.Failure(createResult.ErrorCode, createResult.ErrorMessage);

        var user = createResult.Value!;

        // 3. Persist
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result<UserResponse>.Success(new UserResponse(user.Id, user.Name, user.AuthId));
    }
}
