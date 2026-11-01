// <copyright file="GuidV7ValueGeneratorFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework;

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

/// <summary>
/// Factory for the <see cref="GuidV7ValueGenerator"/>.
/// </summary>
/// <remarks>
/// The value generator is configured by the type of this factory instead of a lambda,
/// because compiled models can only reference value generator factories by type.
/// </remarks>
public class GuidV7ValueGeneratorFactory : ValueGeneratorFactory
{
    /// <inheritdoc />
    public override ValueGenerator Create(IProperty property, ITypeBase typeBase)
    {
        return new GuidV7ValueGenerator();
    }
}
