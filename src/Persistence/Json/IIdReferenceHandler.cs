// <copyright file="IIdReferenceHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json;

using System.Text.Json.Serialization;

/// <summary>
/// Interface for a <see cref="ReferenceHandler"/> which provides the created resolver.
/// </summary>
public interface IIdReferenceHandler
{
    /// <summary>
    /// Gets the created resolver.
    /// </summary>
    ReferenceResolver? Current { get; }
}