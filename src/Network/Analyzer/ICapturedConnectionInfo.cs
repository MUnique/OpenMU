// <copyright file="ICapturedConnectionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Information about a connection of a server, whose traffic can be captured.
/// </summary>
/// <remarks>
/// The implementations are provided by the servers themselves, so that the network connection
/// stays inside the server which owns it.
/// </remarks>
public interface ICapturedConnectionInfo
{
    /// <summary>
    /// Gets the identifier of the connection.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the type of the server which handles this connection.
    /// </summary>
    ServerType ServerType { get; }

    /// <summary>
    /// Gets the identifier of the server which handles this connection.
    /// </summary>
    int ServerId { get; }

    /// <summary>
    /// Gets the description of the server which handles this connection.
    /// </summary>
    string ServerDescription { get; }

    /// <summary>
    /// Gets the name of the account, if the client is logged in.
    /// </summary>
    string? AccountName { get; }

    /// <summary>
    /// Gets the name of the selected character, if one is selected.
    /// </summary>
    string? CharacterName { get; }

    /// <summary>
    /// Gets the remote endpoint of the connection.
    /// </summary>
    string? RemoteEndPoint { get; }

    /// <summary>
    /// Gets the client version which currently applies to this connection.
    /// </summary>
    ClientVersion ClientVersion { get; }

    /// <summary>
    /// Gets the set of packet definitions which applies to this connection.
    /// </summary>
    PacketDefinitionSet DefinitionSet { get; }

    /// <summary>
    /// Gets a value indicating whether the connection is still connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets the name which should be shown for this connection. It's the character name, the
    /// account name or the remote endpoint - whatever is known.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Adds a sink which gets the data packets of this connection.
    /// </summary>
    /// <param name="sink">The sink.</param>
    void AddCaptureSink(IPacketCaptureSink sink);

    /// <summary>
    /// Removes a previously added sink.
    /// </summary>
    /// <param name="sink">The sink.</param>
    void RemoveCaptureSink(IPacketCaptureSink sink);

    /// <summary>
    /// Disconnects the client of this connection.
    /// </summary>
    /// <returns>The async task.</returns>
    ValueTask DisconnectAsync();
}
