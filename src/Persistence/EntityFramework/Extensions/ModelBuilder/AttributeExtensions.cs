// <copyright file="AttributeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="Attribute"/>-related <see cref="EntityTypeBuilder"/>s.
/// </summary>
internal static class AttributeExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="ConstValueAttribute"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ConstValueAttribute> builder)
    {
        builder.Ignore(c => c.AggregateType);
    }

    /// <summary>
    /// Applies the settings for the <see cref="PowerUpDefinitionValue"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<PowerUpDefinitionValue> builder)
    {
        builder.Ignore(p => p.ConstantValue);
    }
}