// <copyright file="GameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The context of a game server which contains all important configurations and services used by one game server instance.
/// </summary>
public class GameServerContext : GameContext, IGameServerContext
{
    private readonly GameServerDefinition _gameServerDefinition;

    private readonly ConcurrentDictionary<uint, List<Player>> _playersByGuild = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerContext" /> class.
    /// </summary>
    /// <param name="gameServerDefinition">The game server definition.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="eventPublisher">The message publisher.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="mapInitializer">The map initializer.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="dropGenerator">The drop generator.</param>
    public GameServerContext(
        GameServerDefinition gameServerDefinition,
        IGuildServer guildServer,
        IEventPublisher eventPublisher,
        ILoginServer loginServer,
        IFriendServer friendServer,
        IPersistenceContextProvider persistenceContextProvider,
        IMapInitializer mapInitializer,
        ILoggerFactory loggerFactory,
        PlugInManager plugInManager,
        IDropGenerator dropGenerator)
        : base(
            gameServerDefinition.GameConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a GameConfiguration"),
            persistenceContextProvider,
            mapInitializer,
            loggerFactory,
            plugInManager,
            dropGenerator)
    {
        this._gameServerDefinition = gameServerDefinition;
        this.Id = gameServerDefinition.ServerID;
        this.GuildServer = guildServer;
        this.EventPublisher = eventPublisher;
        this.LoginServer = loginServer;
        this.FriendServer = friendServer;
        this.ServerConfiguration = gameServerDefinition.ServerConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a ServerConfiguration");
    }

    /// <summary>
    /// Occurs when a guild has been deleted.
    /// </summary>
    public event EventHandler<GuildDeletedEventArgs>? GuildDeleted;

    /// <inheritdoc/>
    public byte Id { get; }

    /// <inheritdoc/>
    public IGuildServer GuildServer { get; } // TODO: Use DI where this is required

    /// <inheritdoc/>
    public IEventPublisher EventPublisher { get; } // TODO: Use DI where this is required, make this private

    /// <inheritdoc/>
    public ILoginServer LoginServer { get; } // TODO: Use DI where this is required

    /// <inheritdoc/>
    public IFriendServer FriendServer { get; } // TODO: Use DI where this is required

    /// <inheritdoc/>
    public GameServerConfiguration ServerConfiguration { get; }

    /// <inheritdoc />
    public override float ExperienceRate => base.ExperienceRate * this._gameServerDefinition.ExperienceRate;

    /// <inheritdoc />
    public void ForEachGuildPlayer(uint guildId, Action<Player> action)
    {
        if (!this._playersByGuild.TryGetValue(guildId, out var playerList))
        {
            return;
        }

        lock (playerList)
        {
            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                var player = playerList[i];
                action(player);
            }
        }
    }

    /// <inheritdoc />
    public void ForEachAlliancePlayer(uint guildId, Action<Player> action)
    {
        if (!this._playersByGuild.TryGetValue(guildId, out var playerList))
        {
            return;
        }

        // TODO: iterate other guilds of the alliance as well; maybe introduce another dictionary with alliance players

        lock (playerList)
        {
            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                var player = playerList[i];
                action(player);
            }
        }
    }

    /// <inheritdoc/>
    public override void AddPlayer(Player player)
    {
        base.AddPlayer(player);
        player.PlayerLeftWorld += this.PlayerLeftWorld;
        player.PlayerEnteredWorld += this.PlayerEnteredWorld;
    }

    /// <inheritdoc/>
    public override void RemovePlayer(Player player)
    {
        player.PlayerEnteredWorld -= this.PlayerEnteredWorld;
        player.PlayerLeftWorld -= this.PlayerLeftWorld;
        base.RemovePlayer(player);
    }

    /// <inheritdoc />
    public void RemoveGuild(uint guildId)
    {
        this._playersByGuild.Remove(guildId, out _);
        this.GuildDeleted?.Invoke(this, new GuildDeletedEventArgs(guildId));
    }

    /// <inheritdoc />
    public void RegisterGuildMember(Player guildMember)
    {
        if (guildMember.GuildStatus is null)
        {
            return;
        }

        var guildId = guildMember.GuildStatus.GuildId;
        var guildList = this._playersByGuild.GetOrAdd(guildId, id => new List<Player>());
        lock (guildList)
        {
            guildList.Add(guildMember);
        }
    }

    /// <inheritdoc />
    public void UnregisterGuildMember(Player guildMember)
    {
        if (guildMember.GuildStatus is null)
        {
            return;
        }

        var guildId = guildMember.GuildStatus.GuildId;
        if (!this._playersByGuild.TryGetValue(guildId, out var guildList))
        {
            return;
        }

        lock (guildList)
        {
            guildList.Remove(guildMember);
        }
    }

    private void PlayerEnteredWorld(object? sender, EventArgs e)
    {
        if (sender is not Player { SelectedCharacter: { } selectedCharacter })
        {
            return;
        }

        this.EventPublisher.PlayerEnteredGame(this.Id, selectedCharacter.Id, selectedCharacter.Name);
    }

    private void PlayerLeftWorld(object? sender, EventArgs e)
    {
        if (sender is not Player { SelectedCharacter: { } selectedCharacter } player)
        {
            return;
        }

        this.EventPublisher.PlayerLeftGame(this.Id, selectedCharacter.Id, selectedCharacter.Name, player.GuildStatus?.GuildId ?? 0);

        if (player.GuildStatus is null)
        {
            return;
        }

        this.UnregisterGuildMember(player);
        player.GuildStatus = null;
    }
}