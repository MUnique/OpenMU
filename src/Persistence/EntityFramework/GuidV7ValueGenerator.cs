// <copyright file="GuidV7ValueGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

/// <summary>
/// A value generator for UUID V7 used as primary key.
/// </summary>
public class GuidV7ValueGenerator : ValueGenerator<Guid>
{
    /// <inheritdoc/>
    public override bool GeneratesTemporaryValues => false;

    /// <inheritdoc/>
    public override Guid Next(EntityEntry entry)
    {
        return GuidV7.NewGuid();
    }
}