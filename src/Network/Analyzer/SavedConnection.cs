// <copyright file="SavedConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System.ComponentModel;
    using System.IO;

    /// <summary>
    /// A <see cref="ICapturedConnection"/> which has been saved in a file and can be loaded again for analyzing.
    /// </summary>
    public class SavedConnection : ICapturedConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedConnection"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public SavedConnection(string filePath)
        {
            this.PacketList = new BindingList<Packet>();
            this.PacketList.LoadFromFile(filePath);
            this.Name = new FileInfo(filePath).Name;
        }

        /// <summary>
        /// Gets the name of the connection.
        /// In this case it's simply the file name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the packets of the connection.
        /// </summary>
        public BindingList<Packet> PacketList { get; }
    }
}