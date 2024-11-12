// <copyright file="ClientVersionResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// A resolver for client Versions.
/// </summary>
public static class ClientVersionResolver
{
    private static readonly IDictionary<long, ClientVersion> Versions = new Dictionary<long, ClientVersion>();

    private static readonly IDictionary<ClientVersion, byte[]> VersionBytes = new Dictionary<ClientVersion, byte[]>();

    /// <summary>
    /// Gets or sets the default version.
    /// </summary>
    public static ClientVersion DefaultVersion { get; set; } = new (6, 3, ClientLanguage.English);

    /// <summary>
    /// Registers the specified version bytes.
    /// </summary>
    /// <param name="versionBytes">The version bytes.</param>
    /// <param name="clientVersion">The client version.</param>
    public static void Register(Span<byte> versionBytes, ClientVersion clientVersion)
    {
        long key = 0;
        if (versionBytes.Length >= 5)
        {
            key = CalculateVersionValue(versionBytes);
        }

        Versions[key] = clientVersion;
        if (!VersionBytes.ContainsKey(clientVersion))
        {
            VersionBytes.Add(clientVersion, versionBytes.ToArray());
        }
    }

    /// <summary>
    /// Resolves the specified version into the default version byte array.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns>The default version bytes for the specified <see cref="ClientVersion"/>.</returns>
    public static byte[] Resolve(ClientVersion version)
    {
        return VersionBytes[version];
    }

    /// <summary>
    /// Resolves the specified version byte array into a <see cref="ClientVersion"/>.
    /// </summary>
    /// <param name="version">The version.</param>
    /// <returns>The resolved client version.</returns>
    public static ClientVersion Resolve(Span<byte> version)
    {
        var versionValue = CalculateVersionValue(version);
        if (Versions.TryGetValue(versionValue, out var clientVersion))
        {
            return clientVersion;
        }

        return DefaultVersion;
    }

    private static long CalculateVersionValue(Span<byte> versionBytes) => (versionBytes.MakeDwordSmallEndian(0) * 0x100L) + versionBytes[4];
}