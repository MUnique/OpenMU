// <copyright file="LetterExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{LetterBody}"/> and <see cref="EntityTypeBuilder{LetterHeader}"/>.
/// </summary>
internal static class LetterExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="LetterBody"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<LetterBody> builder)
    {
        builder.HasOne(body => body.RawHeader);
    }

    /// <summary>
    /// Applies the settings for the <see cref="LetterHeader"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<LetterHeader> builder)
    {
        builder.Ignore(header => header.ReceiverName);
    }
}