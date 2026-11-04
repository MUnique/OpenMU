// <copyright file="IAttribute.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AttributeSystem;

/// <summary>
/// The interface of an attribute.
/// </summary>
public interface IAttribute : IElement
{
    /// <summary>
    /// Gets the attribute definition.
    /// </summary>
    AttributeDefinition Definition { get; }
}