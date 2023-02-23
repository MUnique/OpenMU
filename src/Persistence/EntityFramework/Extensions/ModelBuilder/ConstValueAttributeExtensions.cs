namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class ConstValueAttributeExtensions
{
    public static void Apply(this EntityTypeBuilder<ConstValueAttribute> builder)
    {
        builder.Ignore(c => c.AggregateType);
    }
}