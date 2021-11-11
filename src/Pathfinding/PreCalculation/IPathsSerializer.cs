// <copyright file="IPathsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.IO;

/// <summary>
/// Interface for a paths serializer.
/// </summary>
internal interface IPathsSerializer
{
    /// <summary>
    /// Deserializes the path infos from the specified source.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns>The path infos.</returns>
    IEnumerable<PathInfo> Deserialize(Stream source);

    /// <summary>
    /// Serializes the specified path infos into the stream.
    /// </summary>
    /// <param name="pathInfos">The path infos.</param>
    /// <param name="targetStream">The target stream.</param>
    void Serialize(IEnumerable<PathInfo> pathInfos, Stream targetStream);
}