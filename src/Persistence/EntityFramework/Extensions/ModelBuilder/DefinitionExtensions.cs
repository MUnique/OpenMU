namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class DefinitionExtensions
{
    /// <summary>
    /// Extension for MonsterDefinition entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<MonsterDefinition> builder)
    {
        builder.HasMany<QuestDefinition>().WithOne(q => q.RawQuestGiver);
    }

    /// <summary>
    /// Extension for ItemBasePowerUpDefinition entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<ItemBasePowerUpDefinition> builder)
    {
        builder.Ignore(d => d.BaseValueElement);
    }

    /// <summary>
    /// Extension for PowerUpDefinitionValue entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<PowerUpDefinitionValue> builder)
    {
        builder.Ignore(p => p.ConstantValue);
    }

    /// <summary>
    /// Extension for MasterSkillDefinition entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<MasterSkillDefinition> builder)
    {
        builder.HasOne(s => s.RawRoot);
    }
}