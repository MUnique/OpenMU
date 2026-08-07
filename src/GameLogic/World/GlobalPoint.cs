// <copyright file="GlobalPoint.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.World;

/// <summary>
/// A position in the global world coordinate space.
/// </summary>
public readonly record struct GlobalPoint(ushort X, ushort Y)
{
    /// <summary>
    /// Converts this global position to its chunk and position inside that chunk.
    /// </summary>
    /// <returns>The containing chunk and the local position.</returns>
    public (ChunkId Chunk, LocalPoint Local) ToChunkLocal()
    {
        return (
            new ChunkId((ushort)(this.X / ChunkId.Size), (ushort)(this.Y / ChunkId.Size)),
            new LocalPoint((byte)(this.X % ChunkId.Size), (byte)(this.Y % ChunkId.Size)));
    }
}
