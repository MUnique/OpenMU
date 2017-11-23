// <copyright file="CompactPathsSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Serializes the path infos into a more compact format.
    /// This format uses just 1 byte for end and 1 byte for next step.
    /// It can only be used for maximumRange lower than 8, because of the space limitation.
    /// </summary>
    internal class CompactPathsSerializer : IPathsSerializer
    {
        /// <inheritdoc/>
        public IEnumerable<PathInfo> Deserialize(Stream source)
        {
            const int elementSize = 6;

            // ReSharper disable once TooWideLocalVariableScope we want to reduce stack allocations
            PathInfo pathInfo;
            while (source.Position + elementSize < source.Length)
            {
                byte startX = (byte)source.ReadByte();
                byte startY = (byte)source.ReadByte();
                byte startEndDiff = (byte)source.ReadByte();
                byte startNextStepDiff = (byte)source.ReadByte();
                pathInfo.Combination.Start = new Point(startX, startY);
                byte xOffset = (byte)(startEndDiff >> 4 & 0x0F);
                byte yOffset = (byte)(startEndDiff & 0x0F);
                pathInfo.Combination.End = new Point((byte)(startX + xOffset), (byte)(startY + yOffset));

                byte xOffsetNext = (byte)(startNextStepDiff >> 4 & 0x0F);
                byte yOffsetNext = (byte)(startNextStepDiff & 0x0F);
                pathInfo.NextStep = new Point((byte)(startX + xOffsetNext), (byte)(startY + yOffsetNext));
                yield return pathInfo;
            }
        }

        /// <inheritdoc/>
        public void Serialize(IEnumerable<PathInfo> pathInfos, Stream targetStream)
        {
            foreach (var info in pathInfos)
            {
                targetStream.WriteByte(info.Combination.Start.X);
                targetStream.WriteByte(info.Combination.Start.Y);
                targetStream.WriteByte(CalcDiff(info.Combination.Start, info.Combination.End));
                targetStream.WriteByte(CalcDiff(info.Combination.Start, info.NextStep));
            }
        }

        private static byte CalcDiff(Point start, Point end)
        {
            int diffX = end.X - start.X + 8;
            int diffY = end.Y - start.Y + 8;
            if (diffX > 15)
            {
                throw new IndexOutOfRangeException("diffX");
            }

            if (diffY > 15)
            {
                throw new IndexOutOfRangeException("diffY");
            }

            return (byte)(((diffX << 4) & 0xF0) | (diffY & 0x0F));
        }
    }
}
