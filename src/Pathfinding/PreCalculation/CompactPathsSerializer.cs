// <copyright file="CompactPathsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation;

using System.IO;
using System.Text;

    /// <summary>
    /// Serializes the path infos into a more compact format.
    /// The start point uses 2 bytes per coordinate (ushort), the end and next step are stored
    /// as 4-bit offsets relative to the start point.
    /// It can only be used for maximumRange lower than 8, because of the space limitation.
    /// </summary>
    internal class CompactPathsSerializer : IPathsSerializer
    {
        /// <inheritdoc/>
        public IEnumerable<PathInfo> Deserialize(Stream source)
        {
            using var reader = new BinaryReader(source, Encoding.UTF8, leaveOpen: true);
            while (source.Position + 6 <= source.Length)
            {
                ushort startX = reader.ReadUInt16();
                ushort startY = reader.ReadUInt16();
                byte startEndDiff = reader.ReadByte();
                byte startNextStepDiff = reader.ReadByte();
                var start = new Point(startX, startY);
                byte xOffset = (byte)(startEndDiff >> 4 & 0x0F);
                byte yOffset = (byte)(startEndDiff & 0x0F);
                var end = new Point((ushort)(startX + xOffset), (ushort)(startY + yOffset));

                byte xOffsetNext = (byte)(startNextStepDiff >> 4 & 0x0F);
                byte yOffsetNext = (byte)(startNextStepDiff & 0x0F);
                var nextStep = new Point((ushort)(startX + xOffsetNext), (ushort)(startY + yOffsetNext));
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
                writer.Write(CalcDiff(info.Combination.Start, info.Combination.End));
                writer.Write(CalcDiff(info.Combination.Start, info.NextStep));
            }
        }

    private static byte CalcDiff(Point start, Point end)
    {
        int diffX = end.X - start.X + 8;
        int diffY = end.Y - start.Y + 8;
        if (diffX > 15)
        {
            throw new ArgumentException($"The difference between start and end in the x value is greater than the allowed 15. start: {start}, end: {end}");
        }

        if (diffY > 15)
        {
            throw new ArgumentException($"The difference between start and end in the y value is greater than the allowed 15. start: {start}, end: {end}");
        }

        return (byte)(((diffX << 4) & 0xF0) | (diffY & 0x0F));
    }
}
