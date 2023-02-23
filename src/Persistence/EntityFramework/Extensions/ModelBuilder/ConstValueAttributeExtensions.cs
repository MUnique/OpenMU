namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class ConstValueAttributeExtensions
{
    /// <summary>
    /// Extension for ConstValueAttribute entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<ConstValueAttribute> builder)
    {
        builder.Ignore(c => c.AggregateType);
    }
}