using Microsoft.EntityFrameworkCore;
using Raga.Domain.UserGroups;
using Raga.Domain.Users;

namespace Raga.Infrastructure.Persistence;

/// <summary>
/// Main EF Core DbContext for Raga Bolo.
/// Entity configurations and DbSets will be added per feature.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserGroup> UserGroups => Set<UserGroup>();
    public DbSet<UserGroupMember> UserGroupMembers => Set<UserGroupMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Applies all IEntityTypeConfiguration<T> classes in this assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
