namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class LevelBonusExtensions
{
    public static void Apply(this EntityTypeBuilder<LevelBonus> builder)
    {
        builder.Ignore(l => l.AdditionalValueElement);
    }
}