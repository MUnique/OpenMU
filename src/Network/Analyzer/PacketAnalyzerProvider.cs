// <copyright file="PacketAnalyzerProvider.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.Collections.Concurrent;

/// <summary>
/// Provides a <see cref="PacketAnalyzer"/> per <see cref="PacketDefinitionSet"/>.
/// </summary>
/// <remarks>
/// A <see cref="PacketAnalyzer"/> reads its definitions from the file system, so it's created
/// once per definition set and then shared. The client version is passed per call, so one
/// instance can analyze the traffic of all connections of its definition set.
/// </remarks>
public sealed class PacketAnalyzerProvider : IDisposable
{
    private readonly ConcurrentDictionary<PacketDefinitionSet, PacketAnalyzer> _analyzers = new();

    /// <summary>
    /// Gets the analyzer for the specified definition set.
    /// </summary>
    /// <param name="definitionSet">The definition set.</param>
    /// <returns>The analyzer for the specified definition set.</returns>
    public PacketAnalyzer GetAnalyzer(PacketDefinitionSet definitionSet)
    {
        return this._analyzers.GetOrAdd(definitionSet, set => new PacketAnalyzer(set));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        foreach (var analyzer in this._analyzers.Values)
        {
            analyzer.Dispose();
        }

        this._analyzers.Clear();
    }
}
