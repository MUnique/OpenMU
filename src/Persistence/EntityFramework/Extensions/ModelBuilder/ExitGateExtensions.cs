namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class ExitGateExtensions
{
    public static void Apply(this EntityTypeBuilder<ExitGate> builder)
    {
        builder.HasOne(gate => gate.RawMap);
    }
}