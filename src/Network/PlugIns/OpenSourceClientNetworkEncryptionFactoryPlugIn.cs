// <copyright file="Season106Episode3NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns;

using System.IO.Pipelines;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.SimpleModulus;
using MUnique.OpenMU.Network.Xor;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which provides network encryptors and decryptors for the english open source game clients of season 6 episode 3, version 2.04d
/// </summary>
[PlugIn("Network Encryption - Season 6 Episode 3, Open Source Client", "A plugin which provides network encryptors and decryptors for the english open source game clients of season 6 episode 3, version 2.04d")]
[Guid("AB9EBD28-7A45-4FBA-A282-E2120E70FF17")]
public class OpenSourceClientNetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
{
    /// <inheritdoc />
    public ClientVersion Key { get; } = new (106, 3, ClientLanguage.English);

    /// <inheritdoc />
    public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction)
    {
        if (direction == DataDirection.ServerToClient)
        {
            return new PipelinedEncryptor(target);
        }

        return new PipelinedXor32Encryptor(new PipelinedSimpleModulusEncryptor(target, PipelinedSimpleModulusEncryptor.DefaultClientKey).Writer);
    }

    /// <inheritdoc />
    public IPipelinedDecryptor CreateDecryptor(PipeReader source, DataDirection direction)
    {
        if (direction == DataDirection.ClientToServer)
        {
            return new PipelinedDecryptor(source);
        }

        return new PipelinedSimpleModulusDecryptor(source, PipelinedSimpleModulusDecryptor.DefaultClientKey);
    }
}