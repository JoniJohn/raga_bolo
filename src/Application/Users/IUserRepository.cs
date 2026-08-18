using Raga.Domain.Users;

namespace Raga.Application.Users;

public interface IUserRepository
{
    Task<bool> AuthIdExistsAsync(Guid authId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(long userId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
