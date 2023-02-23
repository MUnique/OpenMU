using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

internal static class MonsterSpawnAreaExtensions
{
    public static void Apply(this EntityTypeBuilder<MonsterSpawnArea> builder)
    {
        builder.HasOne(spawn => spawn.RawMonsterDefinition);
        builder.HasOne(spawn => spawn.RawGameMap);
    }
}