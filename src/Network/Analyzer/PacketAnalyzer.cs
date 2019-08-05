// <copyright file="PacketAnalyzer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Analyzer which analyzes data packets by considering the configuration files.
    /// </summary>
    public sealed class PacketAnalyzer : IDisposable
    {
        private const string ClientToServerPacketsFile = "ClientToServerPackets.xml";
        private const string ServerToClientPacketsFile = "ServerToClientPackets.xml";
        private readonly IList<IDisposable> watchers = new List<IDisposable>();
        private PacketDefinitions clientPacketDefinitions;
        private PacketDefinitions serverPacketDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketAnalyzer"/> class.
        /// The configuration is automatically loaded from the configuration files.
        /// </summary>
        public PacketAnalyzer()
        {
            this.LoadAndWatchConfiguration(def => this.serverPacketDefinitions = def, ServerToClientPacketsFile);
            this.LoadAndWatchConfiguration(def => this.clientPacketDefinitions = def, ClientToServerPacketsFile);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketAnalyzer"/> class
        /// with the specified configurations.
        /// </summary>
        /// <param name="clientPacketDefinitions">The client packet definitions.</param>
        /// <param name="serverPacketDefinitions">The server packet definitions.</param>
        public PacketAnalyzer(PacketDefinitions clientPacketDefinitions, PacketDefinitions serverPacketDefinitions)
        {
            this.clientPacketDefinitions = clientPacketDefinitions;
            this.serverPacketDefinitions = serverPacketDefinitions;
        }

        /// <summary>
        /// Extracts the information of the packet and returns it as a formatted string.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The formatted string with the extracted information.</returns>
        public string ExtractInformation(Packet packet)
        {
            var definitions = packet.ToServer ? this.clientPacketDefinitions : this.serverPacketDefinitions;
            var definition = definitions.Packets.FirstOrDefault(p => (byte)p.Type == packet.Type && p.Code == packet.Code && (!p.SubCodeSpecified || p.SubCode == packet.SubCode));
            if (definition != null)
            {
                var stringBuilder = new StringBuilder()
                    .Append(definition.Name);
                foreach (var field in definition.Fields)
                {
                    stringBuilder.Append(Environment.NewLine)
                        .Append(field.Name).Append(": ").Append(packet.ExtractFieldValue(field));
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            foreach (var watcher in this.watchers)
            {
                watcher.Dispose();
            }

            this.watchers.Clear();
        }

        private void LoadAndWatchConfiguration(Action<PacketDefinitions> assignAction, string fileName)
        {
            assignAction(PacketDefinitions.Load(fileName));
            var watcher = new FileSystemWatcher(Environment.CurrentDirectory, fileName);

            watcher.Changed += (sender, args) =>
            {
                PacketDefinitions definitions;
                try
                {
                    definitions = PacketDefinitions.Load(fileName);
                }
                catch
                {
                    // I know, bad practice... but when it fails, because of some invalid xml file, we just don't assign it.
                    return;
                }

                if (definitions != null)
                {
                    assignAction(definitions);
                }
            };

            watcher.EnableRaisingEvents = true;

            this.watchers.Add(watcher);
        }
    }
}