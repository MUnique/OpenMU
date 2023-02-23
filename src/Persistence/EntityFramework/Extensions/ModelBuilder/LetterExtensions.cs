namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

internal static class LetterExtensions
{
    /// <summary>
    /// Extension for LetterBody entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<LetterBody> builder)
    {
        builder.HasOne(body => body.RawHeader);
    }

    /// <summary>
    /// Extension for LetterHeader entity.
    /// centralize logic for apply configuration on database with code first.
    /// </summary>
    public static void Apply(this EntityTypeBuilder<LetterHeader> builder)
    {
        builder.Ignore(header => header.ReceiverName);
    }
}