// <copyright file="PathsSerializeExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Extension methods for paths serialization.
    /// </summary>
    public static class PathsSerializeExtensions
    {
        /// <summary>
        /// The format in which the path infos will be serialized.
        /// </summary>
        public enum PathInfoFormat
        {
            /// <summary>
            /// The compact format.
            /// This format uses just 1 byte for end and 1 byte for next step.
            /// It can only be used for maximumRange lower than 8, because of the space limitation.
            /// </summary>
            Compact,

            /// <summary>
            /// The normal format. Every point uses exactly 2 bytes.
            /// </summary>
            Normal,
        }

        /// <summary>
        /// Serializes the path infos to the target stream.
        /// </summary>
        /// <param name="pathInfos">The path infos.</param>
        /// <param name="target">The target stream.</param>
        /// <param name="format">The format.</param>
        public static void SerializeToStream(this IEnumerable<PathInfo> pathInfos, Stream target, PathInfoFormat format)
        {
            IPathsSerializer serializer = format == PathInfoFormat.Compact ? new CompactPathsSerializer() as IPathsSerializer : new NormalPathsSerializer();
            target.WriteByte((byte)format);
            serializer.Serialize(pathInfos, target);
        }

        /// <summary>
        /// Deserializes the path infos from the source stream.
        /// </summary>
        /// <param name="source">The source stream.</param>
        /// <returns>The path infos.</returns>
        public static IEnumerable<PathInfo> DeserializeFromStream(this Stream source)
        {
            var format = (PathInfoFormat)source.ReadByte();
            IPathsSerializer serializer = format == PathInfoFormat.Compact ? new CompactPathsSerializer() as IPathsSerializer : new NormalPathsSerializer();
            return serializer.Deserialize(source);
        }
    }
}
