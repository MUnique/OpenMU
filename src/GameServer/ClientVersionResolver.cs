// <copyright file="ClientVersionResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// A resolver for client Versions.
    /// </summary>
    public class ClientVersionResolver
    {
        private static readonly IDictionary<long, ClientVersion> Versions = new Dictionary<long, ClientVersion>();

        private static readonly IDictionary<ClientVersion, byte[]> VersionBytes = new Dictionary<ClientVersion, byte[]>();

        /// <summary>
        /// Registers the specified version bytes.
        /// </summary>
        /// <param name="versionBytes">The version bytes.</param>
        /// <param name="clientVersion">The client version.</param>
        /// <param name="isDefaultForSeasonAndEpisode">if set to <c>true</c>, the version bytes are used as default for season and episode.</param>
        public static void Register(Span<byte> versionBytes, ClientVersion clientVersion, bool isDefaultForSeasonAndEpisode = true)
        {
            Versions.Add(CalculateVersionValue(versionBytes), clientVersion);
            if (isDefaultForSeasonAndEpisode)
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
            if (Versions.TryGetValue(CalculateVersionValue(version), out var clientVersion))
            {
                return clientVersion;
            }

            return new ClientVersion(6, 3, ClientLanguage.English);
        }

        private static long CalculateVersionValue(Span<byte> versionBytes) => (versionBytes.MakeDwordSmallEndian(0) * 0x100) + versionBytes[4];
    }
}