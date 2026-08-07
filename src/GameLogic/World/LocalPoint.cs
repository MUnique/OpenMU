// <copyright file="LocalPoint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.World;

/// <summary>
/// A position inside a world chunk.
/// </summary>
public readonly record struct LocalPoint(byte X, byte Y)
{
    /// <summary>
    /// Converts this local position to global coordinates in the specified chunk.
    /// </summary>
    /// <param name="chunk">The containing chunk.</param>
    /// <returns>The corresponding global position.</returns>
    public GlobalPoint ToGlobal(ChunkId chunk) => chunk.ToGlobal(this);
}
