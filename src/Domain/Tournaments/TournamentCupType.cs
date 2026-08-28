using Raga.Domain.Common;

namespace Raga.Domain.Tournaments;

public sealed class TournamentCupType : BaseEntity
{
    public int Id { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Required by EF Core
    private TournamentCupType() { }

    public TournamentCupType(int id, string code, string name, string? description = null)
    {
        Id = id;
        Code = code;
        Name = name;
        Description = description;
    }
}
