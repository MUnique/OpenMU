// <copyright file="NormalPathsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.IO;
using System.Text;

    /// <summary>
    /// Serializes the path infos into the normal format. Every coordinate uses exactly 2 bytes (ushort).
    /// </summary>
    /// <seealso cref="OpenMU.Pathfinding.PreCalculation.IPathsSerializer" />
    internal class NormalPathsSerializer : IPathsSerializer
    {
        private const int CoordinatesPerInfo = 6;

        /// <inheritdoc/>
        public IEnumerable<PathInfo> Deserialize(Stream source)
        {
            using var reader = new BinaryReader(source, Encoding.UTF8, leaveOpen: true);
            var elementSize = CoordinatesPerInfo * sizeof(ushort);
            while (source.Position + elementSize <= source.Length)
            {
                var start = new Point(reader.ReadUInt16(), reader.ReadUInt16());
                var end = new Point(reader.ReadUInt16(), reader.ReadUInt16());
                var nextStep = new Point(reader.ReadUInt16(), reader.ReadUInt16());
                yield return new PathInfo(new PointCombination(start, end), nextStep);
            }
        }

        /// <inheritdoc/>
        public void Serialize(IEnumerable<PathInfo> pathInfos, Stream targetStream)
        {
            using var writer = new BinaryWriter(targetStream, Encoding.UTF8, leaveOpen: true);
            foreach (var info in pathInfos)
            {
                writer.Write(info.Combination.Start.X);
                writer.Write(info.Combination.Start.Y);
                writer.Write(info.Combination.End.X);
                writer.Write(info.Combination.End.Y);
                writer.Write(info.NextStep.X);
                writer.Write(info.NextStep.Y);
            }
        }
    }
