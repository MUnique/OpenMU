// <copyright file="NormalPathsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Serializes the path infos into the normal format. Every point uses exactly 2 bytes.
    /// </summary>
    /// <seealso cref="OpenMU.Pathfinding.PreCalculation.IPathsSerializer" />
    internal class NormalPathsSerializer : IPathsSerializer
    {
        /// <inheritdoc/>
        public IEnumerable<PathInfo> Deserialize(Stream source)
        {
            const int elementSize = 8;
            while (source.Position + elementSize < source.Length)
            {
                var start = new Point((byte)source.ReadByte(), (byte)source.ReadByte());
                var end = new Point((byte)source.ReadByte(), (byte)source.ReadByte());
                var nextStep = new Point((byte)source.ReadByte(), (byte)source.ReadByte());
                yield return new PathInfo(new PointCombination(start, end), nextStep);
            }
        }

        /// <inheritdoc/>
        public void Serialize(IEnumerable<PathInfo> pathInfos, Stream targetStream)
        {
            foreach (var info in pathInfos)
            {
                targetStream.WriteByte(info.Combination.Start.X);
                targetStream.WriteByte(info.Combination.Start.Y);
                targetStream.WriteByte(info.Combination.End.X);
                targetStream.WriteByte(info.Combination.End.Y);
                targetStream.WriteByte(info.NextStep.X);
                targetStream.WriteByte(info.NextStep.Y);
            }
        }
    }
}
