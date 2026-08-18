using Raga.Domain.Common;
using Raga.Domain.Users.Rules;

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
        var requiredRule = new UserNameRequiredRule(name);
        if (requiredRule.IsBroken())
            return Result<User>.Failure(requiredRule.ErrorCode, requiredRule.Message);

        var maxLengthRule = new UserNameMaxLengthRule(name);
        if (maxLengthRule.IsBroken())
            return Result<User>.Failure(maxLengthRule.ErrorCode, maxLengthRule.Message);

        return Result<User>.Success(new User(name, authId));
    }
}
