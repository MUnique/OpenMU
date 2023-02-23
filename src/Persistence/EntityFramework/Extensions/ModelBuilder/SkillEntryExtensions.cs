namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using MUnique.OpenMU.Persistence.EntityFramework.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal static class SkillEntryExtensions
{
    /// <summary>
    /// Extension for SkillEntry entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<SkillEntry> builder)
    {
        builder.Ignore(s => s.PowerUps);
        builder.Ignore(s => s.PowerUpDuration);
    }
}