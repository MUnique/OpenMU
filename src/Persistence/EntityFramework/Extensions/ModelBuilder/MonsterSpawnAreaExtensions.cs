namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class MonsterSpawnAreaExtensions
{
    /// <summary>
    /// Extension for MonsterSpawnArea entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<MonsterSpawnArea> builder)
    {
        builder.HasOne(spawn => spawn.RawMonsterDefinition);
        builder.HasOne(spawn => spawn.RawGameMap);
    }
}