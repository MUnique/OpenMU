namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class LevelBonusExtensions
{
    /// <summary>
    /// Extension for LevelBonus entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<LevelBonus> builder)
    {
        builder.Ignore(l => l.AdditionalValueElement);
    }
}