namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;
using Microsoft.EntityFrameworkCore;

internal static class GameConfigurationExtensions
{
    public static void Apply(this EntityTypeBuilder<GameConfiguration> builder)
    {
        builder.Property(c => c.ItemDropDuration).HasDefaultValue(TimeSpan.FromSeconds(60));
        builder.Ignore(c => c.ExperienceTable)
            .Ignore(c => c.MasterExperienceTable);
    }
}