using Raga.Domain.Common;

namespace Raga.Domain.UserGroups;

public sealed class UserGroup : BaseEntity
{
    public long Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public long OwnerId { get; private set; }

    // Required by EF Core
    private UserGroup() { }

    private UserGroup(string name, long ownerId)
    {
        Name = name;
        OwnerId = ownerId;
    }

    /// <summary>
    /// Factory method — validates domain rules before constructing the entity.
    /// Owner-exists check is a cross-entity policy handled at the service layer.
    /// </summary>
    public static Result<UserGroup> Create(string name, long ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<UserGroup>.Failure("USER_GROUP_NAME_REQUIRED", "Name is required.");

        if (name.Length > 200)
            return Result<UserGroup>.Failure("USER_GROUP_NAME_TOO_LONG", "Name must not exceed 200 characters.");

        if (ownerId <= 0)
            return Result<UserGroup>.Failure("USER_GROUP_OWNER_INVALID", "Owner ID must be a positive number.");

        return Result<UserGroup>.Success(new UserGroup(name, ownerId));
    }
}
