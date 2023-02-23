namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class GameMapDefinitionExtensions
{
    /// <summary>
    /// Extension for GameMapDefinition entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<GameMapDefinition> builder)
    {
        builder.HasMany(map => map.RawEnterGates);
        builder.HasMany(map => map.RawExitGates).WithOne(g => g.RawMap);
        builder.HasOne(map => map.RawSafezoneMap);
        builder.HasMany(map => map.RawMonsterSpawns);
    }
}