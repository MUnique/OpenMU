// <copyright file="PreSeason6NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.Network.Xor;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which provides network encryptors and decryptors for game clients before season 6.
    /// </summary>
    /// <remarks>
    /// The simple modulus keys were never changed, so the default keys of season 6 work here without changes.
    /// For the Xor32 key it's different. During season 6 on GMO, Webzen desperately changed them in a regular manner - sometimes
    /// every weekly maintenance. Needless to say, calculating the new key was a matter of seconds and provided no protection from skilled hackers at all.
    /// </remarks>
    [PlugIn("Network Encryption - Before Season 6", "A plugin which provides network encryptors and decryptors for game clients before season 6.")]
    [Guid("E72839BF-5ADE-419D-91C5-278EC2A7CEBF")]
    public class PreSeason6NetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
    {
        /// <summary>
        /// The xor32 key for all clients before season 6.
        /// </summary>
        private static readonly byte[] Xor32Key =
        {
            0xE7, 0x6D, 0x3A, 0x89, 0xBC, 0xB2, 0x9F, 0x73, 0x23, 0xA8, 0xFE, 0xB6, 0x49, 0x5D, 0x39, 0x5D, 0x8A, 0xCB, 0x63, 0x8D, 0xEA, 0x7D, 0x2B, 0x5F, 0xC3, 0xB1, 0xE9, 0x83, 0x29, 0x51, 0xE8, 0x56,
        };

        /// <inheritdoc />
        public ClientVersion Key { get; } = default;

        /// <inheritdoc />
        public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction)
        {
            if (direction == DataDirection.ServerToClient)
            {
                return new PipelinedEncryptor(target);
            }

            return new PipelinedXor32Encryptor(new PipelinedSimpleModulusEncryptor(target, PipelinedSimpleModulusEncryptor.DefaultClientKey).Writer, Xor32Key);
        }

        /// <inheritdoc />
        public IPipelinedDecryptor CreateDecryptor(PipeReader source, DataDirection direction)
        {
            if (direction == DataDirection.ClientToServer)
            {
                return new PipelinedDecryptor(source, PipelinedSimpleModulusDecryptor.DefaultServerKey, Xor32Key);
            }

            return new PipelinedSimpleModulusDecryptor(source, PipelinedSimpleModulusDecryptor.DefaultClientKey);
        }
    }
}