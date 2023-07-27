// <copyright file="ExitGateExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Extensions.ModelBuilder;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MUnique.OpenMU.Persistence.EntityFramework.Model;

/// <summary>
/// Extensions for the <see cref="EntityTypeBuilder{ExitGate}"/>.
/// </summary>
internal static class ExitGateExtensions
{
    /// <summary>
    /// Applies the settings for the <see cref="ExitGate"/> entity.
    /// </summary>
    /// <param name="builder">The builder.</param>
    public static void Apply(this EntityTypeBuilder<ExitGate> builder)
    {
        builder.HasOne(gate => gate.RawMap);
    }
}