// <copyright file="INetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A strategy plugin interface for plugins which provide network packet encryptors and decryptors for a specific game client version.
    /// </summary>
    [PlugInPoint("Network Encryption Factories", "Plugins which will be executed when a client connects to a server. It provides network packet encryptors and decryptors for a specific game client version.")]
    [Guid("97DF394A-513B-43AA-A833-40F322FC8E43")]
    public interface INetworkEncryptionFactoryPlugIn : IStrategyPlugIn<ClientVersion>
    {
        /// <summary>
        /// Creates a <see cref="IPipelinedDecryptor"/> for the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="direction">The direction of the data flow.</param>
        /// <returns>The created decryptor.</returns>
        IPipelinedDecryptor CreateDecryptor(PipeReader source, DataDirection direction);

        /// <summary>
        /// Creates a <see cref="IPipelinedEncryptor"/> for the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// /// <param name="direction">The direction of the data flow.</param>
        /// <returns>The created encryptor.</returns>
        IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction);
    }
}
