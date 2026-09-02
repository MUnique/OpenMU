// <copyright file="TestConnectionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// A <see cref="ICapturedConnectionInfo"/> for the tests of the network analyzer components.
/// </summary>
public sealed class TestConnectionInfo : ICapturedConnectionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TestConnectionInfo"/> class.
    /// </summary>
    /// <param name="serverDescription">The description of the server.</param>
    /// <param name="serverId">The identifier of the server.</param>
    public TestConnectionInfo(string serverDescription = "Test Server", int serverId = 1)
    {
        this.ServerDescription = serverDescription;
        this.ServerId = serverId;
    }

    /// <summary>
    /// Gets the sinks which are currently registered.
    /// </summary>
    public IList<IPacketCaptureSink> Sinks { get; } = new List<IPacketCaptureSink>();

    /// <summary>
    /// Gets a value indicating whether <see cref="DisconnectAsync"/> has been called.
    /// </summary>
    public bool IsDisconnectCalled { get; private set; }

    /// <inheritdoc />
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <inheritdoc />
    public ServerType ServerType { get; init; } = ServerType.GameServer;

    /// <inheritdoc />
    public int ServerId { get; }

    /// <inheritdoc />
    public string ServerDescription { get; }

    /// <inheritdoc />
    public string? AccountName { get; init; }

    /// <inheritdoc />
    public string? CharacterName { get; init; }

    /// <inheritdoc />
    public string? RemoteEndPoint { get; init; } = "127.0.0.1:1234";

    /// <inheritdoc />
    public ClientVersion ClientVersion { get; init; } = new(6, 3, ClientLanguage.English);

    /// <inheritdoc />
    public PacketDefinitionSet DefinitionSet { get; init; } = PacketDefinitionSet.GameServer;

    /// <inheritdoc />
    public bool IsConnected => true;

    /// <inheritdoc />
    public string DisplayName => this.CharacterName ?? this.AccountName ?? this.RemoteEndPoint ?? this.Id.ToString();

    /// <inheritdoc />
    public void AddCaptureSink(IPacketCaptureSink sink) => this.Sinks.Add(sink);

    /// <inheritdoc />
    public void RemoveCaptureSink(IPacketCaptureSink sink) => this.Sinks.Remove(sink);

    /// <inheritdoc />
    public ValueTask DisconnectAsync()
    {
        this.IsDisconnectCalled = true;
        return ValueTask.CompletedTask;
    }
}
