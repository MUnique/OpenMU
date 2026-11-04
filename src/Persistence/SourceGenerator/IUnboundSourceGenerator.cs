// <copyright file="IUnboundSourceGenerator.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator;

/// <summary>
/// Interface for a generator which generates sources without depending on a context.
/// </summary>
public interface IUnboundSourceGenerator
{
    /// <summary>
    /// Generates the source files.
    /// </summary>
    /// <returns>The created source files.</returns>
    public IEnumerable<(string Name, string Source)> GenerateSources();
}