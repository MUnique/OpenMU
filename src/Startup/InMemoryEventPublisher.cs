// <copyright file="InMemoryEventPublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// An <see cref="IEventPublisher"/> which publishes directly to the available started servers in this application.
/// </summary>
public class InMemoryEventPublisher : IEventPublisher
{
    private readonly IDictionary<int, IGameServer> _gameServers;
    private readonly IFriendServer _friendServer;
    private readonly IGuildServer _guildServer;

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryEventPublisher"/> class.
    /// </summary>
    /// <param name="gameServers">The game servers.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="guildServer">The guild server.</param>
    public InMemoryEventPublisher(IDictionary<int, IGameServer> gameServers, IFriendServer friendServer, IGuildServer guildServer)
    {
        this._gameServers = gameServers;
        this._friendServer = friendServer;
        this._guildServer = guildServer;
    }

    /// <inheritdoc />
    public void PlayerEnteredGame(byte serverId, Guid characterId, string characterName)
    {
        this._guildServer.PlayerEnteredGame(characterId, characterName, serverId);

        this._friendServer.PlayerEnteredGame(serverId, characterId, characterName);
    }

    /// <inheritdoc />
    public void PlayerLeftGame(byte serverId, Guid characterId, string characterName, uint guildId = 0)
    {
        if (guildId > 0)
        {
            this._guildServer.GuildMemberLeftGame(guildId, characterId, serverId);
        }

        this._friendServer.PlayerLeftGame(characterId, characterName);
    }

    /// <inheritdoc />
    public void GuildMessage(uint guildId, string sender, string message)
    {
        foreach (var gameServer in this._gameServers)
        {
            gameServer.Value.GuildChatMessage(guildId, sender, message);
        }
    }

    /// <inheritdoc />
    public void AllianceMessage(uint guildId, string sender, string message)
    {
        foreach (var gameServer in this._gameServers)
        {
            gameServer.Value.AllianceChatMessage(guildId, sender, message);
        }
    }
}