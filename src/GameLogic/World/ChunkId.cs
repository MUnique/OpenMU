// <copyright file="ChunkId.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.World;

/// <summary>
/// Identifies a 256x256 tile chunk in the global world.
/// </summary>
public readonly record struct ChunkId(ushort X, ushort Y)
{
    /// <summary>
    /// Gets the tile width and height of a chunk.
    /// </summary>
    public const int Size = 256;

    /// <summary>
    /// Converts a local position in this chunk to a global position.
    /// </summary>
    /// <param name="local">The position inside this chunk.</param>
    /// <returns>The corresponding global position.</returns>
    public GlobalPoint ToGlobal(LocalPoint local)
    {
        return new GlobalPoint((ushort)(this.X * Size + local.X), (ushort)(this.Y * Size + local.Y));
    }
}
