using Raga.Domain.Common;
using Raga.Domain.UserGroups.Rules;

namespace Raga.Domain.UserGroups;

public sealed class UserGroup
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
        var nameRequiredRule = new UserGroupNameRequiredRule(name);
        if (nameRequiredRule.IsBroken())
            return Result<UserGroup>.Failure(nameRequiredRule.ErrorCode, nameRequiredRule.Message);

        var nameMaxLengthRule = new UserGroupNameMaxLengthRule(name);
        if (nameMaxLengthRule.IsBroken())
            return Result<UserGroup>.Failure(nameMaxLengthRule.ErrorCode, nameMaxLengthRule.Message);

        var ownerIdValidRule = new UserGroupOwnerIdValidRule(ownerId);
        if (ownerIdValidRule.IsBroken())
            return Result<UserGroup>.Failure(ownerIdValidRule.ErrorCode, ownerIdValidRule.Message);

        return Result<UserGroup>.Success(new UserGroup(name, ownerId));
    }
}
