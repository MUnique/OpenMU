namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using MUnique.OpenMU.Persistence.EntityFramework.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal static class SkillEntryExtensions
{
    public static void Apply(this EntityTypeBuilder<SkillEntry> builder)
    {
        builder.Ignore(s => s.PowerUps);
        builder.Ignore(s => s.PowerUpDuration);
    }
}