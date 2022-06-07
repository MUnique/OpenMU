// <copyright file="GuildChangeToGameServerPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GuildServer;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// A <see cref="IGuildChangePublisher"/> which directly notifies the game server objects.
/// </summary>
public class GuildChangeToGameServerPublisher : IGuildChangePublisher
{
    private readonly IDictionary<int, IGameServer> _gameServers;

    /// <summary>
    /// Initializes a new instance of the <see cref="GuildChangeToGameServerPublisher"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    public GuildChangeToGameServerPublisher(IDictionary<int, IGameServer> gameServers)
    {
        this._gameServers = gameServers;
    }

    /// <inheritdoc />
    public void GuildPlayerKicked(string playerName)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            gameServer.GuildPlayerKicked(playerName);
        }
    }

    /// <inheritdoc />
    public void GuildDeleted(uint guildId)
    {
        foreach (var gameServer in this._gameServers.Values)
        {
            gameServer.GuildDeleted(guildId);
        }
    }

    /// <inheritdoc />
    public void AssignGuildToPlayer(byte serverId, string characterName, GuildMemberStatus status)
    {
        if (this._gameServers.TryGetValue(serverId, out var gameServer))
        {
            gameServer.AssignGuildToPlayer(characterName, status);
        }
    }
}