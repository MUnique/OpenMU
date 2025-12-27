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
    private static readonly uint[] DefaultServerToClientKey =
    {
        72619, 72691, 73136, 78576,
        14067, 5411, 31865, 16225,
        560, 8315, 21196, 27946,
    };

    private static readonly uint[] DefaultClientToServerKey =
    {
        72619, 72691, 73136, 78576,
        22606, 40463, 33209, 33169,
        560, 8315, 21196, 27946,
    };

    private static readonly byte[] Xor32Key = LoadXor32Key();
    private static readonly HackCheckKeys? HackCheckKeySet = LoadHackCheckKeys();
    private static readonly SimpleModulusKeys ServerToClientKey = LoadSimpleModulusKeys("MU_SM_ENC_097", DefaultServerToClientKey, true);
    private static readonly SimpleModulusKeys ClientToServerKey = LoadSimpleModulusKeys("MU_SM_DEC_097", DefaultClientToServerKey, false);

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
                return new PipelinedSimpleModulusEncryptor(hackCheck.Writer, ServerToClientKey);
            }

            return new PipelinedSimpleModulusEncryptor(target, ServerToClientKey);
        }

        if (HackCheckKeySet is { } clientKeys)
        {
            var hackCheck = new PipelinedHackCheckEncryptor(target, clientKeys);
            target = hackCheck.Writer;
        }

        return new PipelinedXor32Encryptor(
            new PipelinedSimpleModulusEncryptor(target, ClientToServerKey).Writer,
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

            return new PipelinedDecryptor(source, ClientToServerKey, Xor32Key);
        }

        if (HackCheckKeySet is { } serverKeys)
        {
            source = new PipelinedHackCheckDecryptor(source, serverKeys).Reader;
        }

        return new PipelinedSimpleModulusDecryptor(source, ServerToClientKey);
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

    private static SimpleModulusKeys LoadSimpleModulusKeys(string envVar, uint[] fallback, bool isEncryption)
    {
        var path = Environment.GetEnvironmentVariable(envVar);
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                var serializer = new SimpleModulusKeySerializer();
                if (serializer.TryDeserialize(path.Trim(), out var mod, out var key, out var xor))
                {
                    var combined = CombineKeys(mod, key, xor);
                    return isEncryption
                        ? SimpleModulusKeys.CreateEncryptionKeys(combined)
                        : SimpleModulusKeys.CreateDecryptionKeys(combined);
                }
            }
            catch
            {
                // Fall back to built-in keys when the file can't be loaded.
            }
        }

        return isEncryption
            ? SimpleModulusKeys.CreateEncryptionKeys(fallback)
            : SimpleModulusKeys.CreateDecryptionKeys(fallback);
    }

    private static uint[] CombineKeys(uint[] modulus, uint[] key, uint[] xor)
    {
        var combined = new uint[modulus.Length + key.Length + xor.Length];
        modulus.CopyTo(combined, 0);
        key.CopyTo(combined, modulus.Length);
        xor.CopyTo(combined, modulus.Length + key.Length);
        return combined;
    }
}
