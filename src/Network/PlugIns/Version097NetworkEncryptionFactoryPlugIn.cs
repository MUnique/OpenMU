// <copyright file="Version097NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns;

using System.Globalization;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.HackCheck;
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
    private static readonly HackCheckKeys? HackCheckKeySet = LoadHackCheckKeys();

    /// <inheritdoc />
    public ClientVersion Key { get; } = new (0, 97, ClientLanguage.English);

    /// <inheritdoc />
    public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction)
    {
        if (direction == DataDirection.ServerToClient)
        {
            // The 0.97 client decrypts server packets with simple modulus only.
            if (HackCheckKeySet is { } keys)
            {
                var hackCheck = new PipelinedHackCheckEncryptor(target, keys);
                return new PipelinedSimpleModulusEncryptor(hackCheck.Writer, PipelinedSimpleModulusEncryptor.DefaultServerKey);
            }

            return new PipelinedSimpleModulusEncryptor(target, PipelinedSimpleModulusEncryptor.DefaultServerKey);
        }

        if (HackCheckKeySet is { } clientKeys)
        {
            var hackCheck = new PipelinedHackCheckEncryptor(target, clientKeys);
            target = hackCheck.Writer;
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
            if (HackCheckKeySet is { } keys)
            {
                source = new PipelinedHackCheckDecryptor(source, keys).Reader;
            }

            return new PipelinedDecryptor(source, PipelinedSimpleModulusDecryptor.DefaultServerKey, Xor32Key);
        }

        if (HackCheckKeySet is { } serverKeys)
        {
            source = new PipelinedHackCheckDecryptor(source, serverKeys).Reader;
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

    private static HackCheckKeys? LoadHackCheckKeys()
    {
        var customerName = Environment.GetEnvironmentVariable("MU_HACKCHECK_NAME_097");
        var clientSerial = Environment.GetEnvironmentVariable("MU_HACKCHECK_SERIAL_097");
        if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(clientSerial))
        {
            return null;
        }

        return HackCheckKeys.Create(customerName.Trim(), clientSerial.Trim());
    }
}
