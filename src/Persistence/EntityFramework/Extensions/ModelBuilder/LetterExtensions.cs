namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class LetterExtensions
{
    public static void Apply(this EntityTypeBuilder<LetterBody> builder)
    {
        builder.HasOne(body => body.RawHeader);
    }

    public static void Apply(this EntityTypeBuilder<LetterHeader> builder)
    {
        builder.Ignore(header => header.ReceiverName);
    }
}