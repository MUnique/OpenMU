// <copyright file="ConfigurableNetworkEncryptionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.ExDbConnector
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Network.Xor;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A configurable network encryption factory plugin which reads the Xor32 key from the ChatServer.cfg file. Only used by the ExDbConnector project.
    /// </summary>
    [PlugIn("Configurable encryption plugin", "A configurable network encryption factory plugin which reads the Xor32 key from the ChatServer.cfg file. Only used by the ExDbConnector project.")]
    [Guid("890997B2-9334-4E9E-8C82-4492A831BCE3")]
    public class ConfigurableNetworkEncryptionPlugIn : INetworkEncryptionFactoryPlugIn
    {
        private readonly byte[] xor32Key;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurableNetworkEncryptionPlugIn"/> class.
        /// </summary>
        public ConfigurableNetworkEncryptionPlugIn()
        {
            var settings = new Settings("ChatServer.cfg");
            this.xor32Key = settings.Xor32Key ?? new byte[32];
        }

        /// <summary>
        /// Gets the version for which this plugin is available.
        /// </summary>
        public static ClientVersion Version { get; } = new (byte.MaxValue, byte.MaxValue, ClientLanguage.Invariant);

        /// <inheritdoc />
        public ClientVersion Key => Version;

        /// <inheritdoc />
        public IPipelinedDecryptor? CreateDecryptor(PipeReader source, DataDirection direction)
        {
            return new PipelinedXor32Decryptor(source, this.xor32Key);
        }

        /// <inheritdoc />
        public IPipelinedEncryptor? CreateEncryptor(PipeWriter target, DataDirection direction)
        {
            // At least until season 6, there is no encryption from server to client.
            // ex700 may require packet twister here.
            return null;
        }
    }
}
