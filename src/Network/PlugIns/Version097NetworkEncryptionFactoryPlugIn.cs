// <copyright file="Version097NetworkEncryptionFactoryPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.PlugIns;

using System.Collections;
using System.Globalization;
using System.IO;
using System.IO.Pipelines;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    private const string DefaultHackCheckCustomerName = "OpenMU97";
    private const string DefaultHackCheckSerial = "TbYehR2hFUPBKgZj";
    private static readonly byte[] DefaultXor32Key097 =
    {
        0xE7, 0x6D, 0x3A, 0x89, 0xBC, 0xB2, 0x9F, 0x73,
        0x23, 0xA8, 0xFE, 0xB6, 0x49, 0x5D, 0x39, 0x5D,
        0x8A, 0xCB, 0x63, 0x8D, 0xEA, 0x7D, 0x2B, 0x5F,
        0xC3, 0xB1, 0xE9, 0x83, 0x29, 0x51, 0xE8, 0x56,
    };

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

    private readonly byte[] _xor32Key;
    private readonly HackCheckKeys _hackCheckKeys;
    private readonly SimpleModulusKeys _serverToClientKey;
    private readonly SimpleModulusKeys _clientToServerKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="Version097NetworkEncryptionFactoryPlugIn"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public Version097NetworkEncryptionFactoryPlugIn(IServiceProvider? serviceProvider = null)
    {
        this._xor32Key = LoadXor32Key();
        this._hackCheckKeys = LoadHackCheckKeys(serviceProvider);
        this._serverToClientKey = LoadSimpleModulusKeys("MU_SM_ENC_097", DefaultServerToClientKey, true, "Enc2.dat");
        this._clientToServerKey = LoadSimpleModulusKeys("MU_SM_DEC_097", DefaultClientToServerKey, false, "Dec1.dat");
    }

    /// <inheritdoc />
    public ClientVersion Key { get; } = new (0, 97, ClientLanguage.English);

    /// <inheritdoc />
    public IPipelinedEncryptor CreateEncryptor(PipeWriter target, DataDirection direction)
    {
        if (direction == DataDirection.ServerToClient)
        {
            // The 0.97 client decrypts server packets with simple modulus only.
            var hackCheck = new PipelinedHackCheckEncryptor(target, this._hackCheckKeys);
            return new PipelinedSimpleModulusEncryptor(hackCheck.Writer, this._serverToClientKey);
        }

        var clientHackCheck = new PipelinedHackCheckEncryptor(target, this._hackCheckKeys);
        target = clientHackCheck.Writer;

        return new PipelinedXor32Encryptor(
            new PipelinedSimpleModulusEncryptor(target, this._clientToServerKey).Writer,
            this._xor32Key);
    }

    /// <inheritdoc />
    public IPipelinedDecryptor CreateDecryptor(PipeReader source, DataDirection direction)
    {
        if (direction == DataDirection.ClientToServer)
        {
            source = new PipelinedHackCheckDecryptor(source, this._hackCheckKeys).Reader;

            return new PipelinedDecryptor(source, this._clientToServerKey, this._xor32Key);
        }

        source = new PipelinedHackCheckDecryptor(source, this._hackCheckKeys).Reader;

        return new PipelinedSimpleModulusDecryptor(source, this._serverToClientKey);
    }

    private static byte[] LoadXor32Key()
    {
        var env = Environment.GetEnvironmentVariable("MU_XOR32_097");
        if (string.IsNullOrWhiteSpace(env))
        {
            return DefaultXor32Key097;
        }

        var parts = env.Split(new[] { ' ', ',', ';', ':' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 32)
        {
            return DefaultXor32Key097;
        }

        var key = new byte[32];
        for (var i = 0; i < parts.Length; i++)
        {
            if (!byte.TryParse(parts[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var value))
            {
                return DefaultXor32Key097;
            }

            key[i] = value;
        }

        return key;
    }

    private static HackCheckKeys LoadHackCheckKeys(IServiceProvider? serviceProvider)
    {
        var customerName = Environment.GetEnvironmentVariable("MU_HACKCHECK_NAME_097");
        var clientSerial = Environment.GetEnvironmentVariable("MU_HACKCHECK_SERIAL_097");
        if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(clientSerial))
        {
            var configuredSerial = TryGetClientSerialFromConfiguration(serviceProvider);
            return HackCheckKeys.Create(DefaultHackCheckCustomerName, configuredSerial ?? DefaultHackCheckSerial);
        }

        return HackCheckKeys.Create(customerName.Trim(), clientSerial.Trim());
    }

    private static string? TryGetClientSerialFromConfiguration(IServiceProvider? serviceProvider)
    {
        if (serviceProvider is null)
        {
            return null;
        }

        var providerType = Type.GetType("MUnique.OpenMU.Persistence.IPersistenceContextProvider, MUnique.OpenMU.Persistence");
        if (providerType is null)
        {
            return null;
        }

        var provider = serviceProvider.GetService(providerType);
        if (provider is null)
        {
            return null;
        }

        var createContext = providerType.GetMethod("CreateNewConfigurationContext", Type.EmptyTypes);
        if (createContext is null)
        {
            return null;
        }

        var context = createContext.Invoke(provider, null);
        if (context is null)
        {
            return null;
        }

        var disposable = context as IDisposable;
        try
        {
            var clientDefinitionType = Type.GetType("MUnique.OpenMU.DataModel.Configuration.GameClientDefinition, MUnique.OpenMU.DataModel");
            if (clientDefinitionType is null)
            {
                return null;
            }

            var getAsync = context.GetType().GetMethod("GetAsync", new[] { typeof(Type), typeof(CancellationToken) });
            if (getAsync is null)
            {
                return null;
            }

            var valueTask = getAsync.Invoke(context, new object[] { clientDefinitionType, CancellationToken.None });
            if (valueTask is null)
            {
                return null;
            }

            var asTask = valueTask.GetType().GetMethod("AsTask");
            if (asTask is null)
            {
                return null;
            }

            var task = asTask.Invoke(valueTask, null) as Task;
            if (task is null)
            {
                return null;
            }

            task.GetAwaiter().GetResult();
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty?.GetValue(task) as IEnumerable;
            if (result is null)
            {
                return null;
            }

            var seasonProperty = clientDefinitionType.GetProperty("Season");
            var episodeProperty = clientDefinitionType.GetProperty("Episode");
            var serialProperty = clientDefinitionType.GetProperty("Serial");
            if (seasonProperty is null || episodeProperty is null || serialProperty is null)
            {
                return null;
            }

            foreach (var client in result)
            {
                if (seasonProperty.GetValue(client) is not byte season || episodeProperty.GetValue(client) is not byte episode)
                {
                    continue;
                }

                if (season != 0 || episode != 97)
                {
                    continue;
                }

                if (serialProperty.GetValue(client) is byte[] serialBytes && serialBytes.Length > 0)
                {
                    return Encoding.ASCII.GetString(serialBytes);
                }
            }
        }
        finally
        {
            disposable?.Dispose();
        }

        return null;
    }

    private static SimpleModulusKeys LoadSimpleModulusKeys(string envVar, uint[] fallback, bool isEncryption, string defaultFileName)
    {
        var path = Environment.GetEnvironmentVariable(envVar);
        if (string.IsNullOrWhiteSpace(path))
        {
            var baseDir = AppContext.BaseDirectory;
            var dataPath = Path.Combine(baseDir, "Data", "Keys097", defaultFileName);
            if (File.Exists(dataPath))
            {
                path = dataPath;
            }
            else
            {
                var localPath = Path.Combine(baseDir, "Keys097", defaultFileName);
                if (File.Exists(localPath))
                {
                    path = localPath;
                }
            }
        }

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
