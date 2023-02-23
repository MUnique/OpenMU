namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class DefinitionExtensions
{
    public static void Apply(this EntityTypeBuilder<MonsterDefinition> builder)
    {
        builder.HasMany<QuestDefinition>().WithOne(q => q.RawQuestGiver);
    }

    public static void Apply(this EntityTypeBuilder<ItemBasePowerUpDefinition> builder)
    {
        builder.Ignore(d => d.BaseValueElement);
    }

    public static void Apply(this EntityTypeBuilder<PowerUpDefinitionValue> builder)
    {
        builder.Ignore(p => p.ConstantValue);
    }

    public static void Apply(this EntityTypeBuilder<MasterSkillDefinition> builder)
    {
        builder.HasOne(s => s.RawRoot);;
    }
}