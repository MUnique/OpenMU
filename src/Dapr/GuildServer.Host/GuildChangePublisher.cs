// <copyright file="GuildChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer.Host;

using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// Publisher for guild changes over Dapr.
/// </summary>
public class GuildChangePublisher : IGuildChangePublisher
{
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildChangePublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    public GuildChangePublisher(DaprClient daprClient)
    {
        this._daprClient = daprClient;
    }

    /// <inheritdoc />
    public void GuildPlayerKicked(string playerName)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildPlayerKicked), playerName);
    }

    /// <inheritdoc />
    public void GuildDeleted(uint guildId)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(IGameServer.GuildDeleted), guildId);
    }

    /// <inheritdoc />
    public void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status)
    {
        this._daprClient.InvokeMethodAsync($"gameServer{serverId + 1}", nameof(IGameServer.AssignGuildToPlayer), new GuildMemberAssignArguments(characterName, status));
    }
}