// <copyright file="PacketDefinitions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Packet definitions for a specific connection direction (e.g. game server to game client).
    /// </summary>
    [XmlType(AnonymousType = true, Namespace = XmlNamespace)]
    [XmlRoot(Namespace = XmlNamespace, IsNullable = false)]
    [Serializable]
    public class PacketDefinitions
    {
        /// <summary>
        /// The XML namespace for this class.
        /// </summary>
        public const string XmlNamespace = "http://www.munique.net/OpenMU/PacketDefinitions";

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the structures.
        /// </summary>
        [XmlArrayItem(IsNullable = false)]
        public Structure[] Structures { get; set; }

        /// <summary>
        /// Gets or sets the packet definitions.
        /// </summary>
        [XmlArray(IsNullable = true)]
        [XmlArrayItem("Packet", IsNullable = false)]
        public PacketDefinition[] Packets { get; set; }

        /// <summary>
        /// Loads the specified file and returns the parsed <see cref="PacketDefinitions"/> object.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The parsed object.</returns>
        public static PacketDefinitions Load(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            using (var fileStream = fileInfo.OpenRead())
            using (var xmlReader = XmlReader.Create(fileStream))
            {
                var serializer = new XmlSerializer(typeof(PacketDefinitions));
                if (!serializer.CanDeserialize(xmlReader))
                {
                    throw new ArgumentException($"File is not expected xml format: {filePath}");
                }

                return (PacketDefinitions)serializer.Deserialize(xmlReader);
            }
        }

        /// <summary>
        /// Saves this instance to the specified file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public void Save(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            var settings = new XmlWriterSettings { Indent = true };
            using (var fileStream = fileInfo.OpenWrite())
            using (var xmlWriter = XmlWriter.Create(fileStream, settings))
            {
                var serializer = new XmlSerializer(typeof(PacketDefinitions));
                serializer.Serialize(xmlWriter, this);
            }
        }
    }
}