// <copyright file="Version097NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns;

using System.Globalization;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.SimpleModulus;
using MUnique.OpenMU.Network.Xor;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin which provides network encryptors and decryptors for game clients of version 0.97.
/// </summary>
[PlugIn("Network Encryption - 0.97", "A plugin which provides network encryptors and decryptors for game clients of version 0.97.")]
[Guid("6B6F07F2-709F-4A39-8897-0B6A2E7DE8C0")]
public class Version097NetworkEncryptionFactoryPlugIn : INetworkEncryptionFactoryPlugIn
{
    private static readonly byte[] Xor32Key = LoadXor32Key();

    /// <inheritdoc />
    public ClientVersion Key { get; } = new (0, 97, ClientLanguage.English);

    /// <inheritdoc />
    public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction)
    {
        if (direction == DataDirection.ServerToClient)
        {
            // The 0.97 client decrypts server packets with simple modulus only.
            return new PipelinedSimpleModulusEncryptor(target, PipelinedSimpleModulusEncryptor.DefaultServerKey);
        }

        return new PipelinedXor32Encryptor(
            new PipelinedSimpleModulusEncryptor(target, PipelinedSimpleModulusEncryptor.DefaultClientKey).Writer,
            Xor32Key);
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

    private static byte[] LoadXor32Key()
    {
        var env = Environment.GetEnvironmentVariable("MU_XOR32_097");
        if (string.IsNullOrWhiteSpace(env))
        {
            return DefaultKeys.Xor32Key;
        }

        var parts = env.Split(new[] { ' ', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 32)
        {
            return DefaultKeys.Xor32Key;
        }

        var key = new byte[32];
        for (var i = 0; i < parts.Length; i++)
        {
            if (!byte.TryParse(parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                return DefaultKeys.Xor32Key;
            }

            key[i] = value;
        }

        return key;
    }
}
