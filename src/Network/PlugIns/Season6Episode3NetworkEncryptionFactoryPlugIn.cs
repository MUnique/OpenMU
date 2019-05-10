// <copyright file="Season6Episode3NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns
{
    using System.IO.Pipelines;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// A plugin which provides network encryptors and decryptors for english game clients of season 6 episode 3, version 1.04d.
    /// </summary>
    [PlugIn("Network Encryption - Season 6 Episode 3, 1.04d, ENG", "A plugin which provides network encryptors and decryptors for english game clients of season 6 episode 3, version 1.04d")]
    [Guid("AC79C81C-36A1-49A0-85AD-E4DAC7D5C5CE")]
    public class Season6Episode3NetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
    {
        /// <inheritdoc />
        public ClientVersion Key { get; } = new ClientVersion(6, 3, ClientLanguage.English);

        /// <inheritdoc />
        public IPipelinedEncryptor CreateEncryptor(PipeWriter target) => new PipelinedEncryptor(target);

        /// <inheritdoc />
        public IPipelinedDecryptor CreateDecryptor(PipeReader source) => new PipelinedDecryptor(source);
    }
}