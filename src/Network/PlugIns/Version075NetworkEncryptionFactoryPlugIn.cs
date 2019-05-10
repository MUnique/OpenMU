// <copyright file="Version075NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which provides network encryptors and decryptors for game clients of version 0.75 (server version 0.65).
    /// </summary>
    /// <remarks>
    /// This early and very old version doesn't seem to have xor 32 encryption yet.
    /// However, it uses simple modulus with some other keys.
    /// </remarks>
    [PlugIn("Network Encryption - 0.75", "A plugin which provides network encryptors and decryptors for game clients of version 0.75 (server version 0.65)")]
    [Guid("E83F05AC-18D4-4714-AEFB-2F4F3B37951C")]
    public class Version075NetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
    {
        /// <summary>
        /// The simple modulus keys which are used to transfer data from client to server.
        /// </summary>
        /// <remarks>
        /// Dec1.dat:
        ///  Loaded Key: 73506, 145721, 182384, 139019, 14411, 11477, 11005, 12448, 45328, 58518, 59318, 64631
        ///  Calculated Key: 73506, 145721, 182384, 139019, 23759, 22905, 32085, 4255, 45328, 58518, 59318, 64631
        /// </remarks>
        private static readonly SimpleModulusKeys ClientToServerKeys = new SimpleModulusKeys(
            new uint[] { 73506, 145721, 182384, 139019 },
            new uint[] { 45328, 58518, 59318, 64631 },
            new uint[] { 23759, 22905, 32085, 4255 },
            new uint[] { 14411, 11477, 11005, 12448 });

        /// <summary>
        /// The simple modulus keys which are used to transfer data from server to client.
        /// </summary>
        /// <remarks>
        /// Enc2.dat:
        ///  Loaded Key: 145227, 98529, 87383, 160709, 28241, 32194, 12558, 30918, 32786, 43397, 14849, 49619
        ///  Calculated Key: 145227, 98529, 87383, 160709, 21701, 23734, 14536, 8956, 32786, 43397, 14849, 49619
        /// </remarks>
        private static readonly SimpleModulusKeys ServerToClientKeys = new SimpleModulusKeys(
            new uint[] { 145227, 98529, 87383, 160709 },
            new uint[] { 32786, 43397, 14849, 49619 },
            new uint[] { 28241, 32194, 12558, 30918 },
            new uint[] { 21701, 23734, 14536, 8956 });

        /// <inheritdoc />
        public ClientVersion Key { get; } = new ClientVersion(0, 75, ClientLanguage.Invariant);

        /// <inheritdoc />
        public IPipelinedEncryptor CreateEncryptor(PipeWriter target) => new PipelinedSimpleModulusEncryptor(target, ServerToClientKeys);

        /// <inheritdoc />
        public IPipelinedDecryptor CreateDecryptor(PipeReader source) => new PipelinedSimpleModulusDecryptor(source, ClientToServerKeys);
    }
}