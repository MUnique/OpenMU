// <copyright file="IConnectServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Collections.Concurrent;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The internal interface of a connect server.
/// </summary>
internal interface IConnectServer
{
    /// <summary>
    /// Gets the connect infos.
    /// </summary>
    ConcurrentDictionary<ushort, byte[]> ConnectInfos { get; }

    /// <summary>
    /// Gets the server list.
    /// </summary>
    ServerList ServerList { get; }

    /// <summary>
    /// Gets the connectServerSettings.
    /// </summary>
    IConnectServerSettings Settings { get; }

    /// <summary>
    /// Gets the client version.
    /// </summary>
    ClientVersion ClientVersion { get; }
}