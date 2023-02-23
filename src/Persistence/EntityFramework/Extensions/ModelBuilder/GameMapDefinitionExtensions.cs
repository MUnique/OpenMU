namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class GameMapDefinitionExtensions
{
    public static void Apply(this EntityTypeBuilder<GameMapDefinition> builder)
    {
        builder.HasMany(map => map.RawEnterGates);
        builder.HasMany(map => map.RawExitGates).WithOne(g => g.RawMap);
        builder.HasOne(map => map.RawSafezoneMap);
        builder.HasMany(map => map.RawMonsterSpawns);
    }
}