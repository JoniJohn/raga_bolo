using Raga.Domain.Common;

namespace Raga.Domain.Users;

public sealed class User : BaseEntity
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid? AuthId { get; private set; }

    // Required by EF Core
    private User() { }

    private User(string name, Guid? authId)
    {
        Name = name;
        AuthId = authId;
    }

    /// <summary>
    /// Factory method — validates domain rules before constructing the entity.
    /// Duplicate-AuthId check is a cross-entity policy handled at the service layer.
    /// </summary>
    public static Result<User> Create(string name, Guid? authId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<User>.Failure("USER_NAME_REQUIRED", "Name is required.");

        if (name.Length > 255)
            return Result<User>.Failure("USER_NAME_TOO_LONG", "Name must not exceed 255 characters.");

        return Result<User>.Success(new User(name, authId));
    }
}
