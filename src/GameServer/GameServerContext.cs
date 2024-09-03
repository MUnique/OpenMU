// <copyright file="GameServerContext.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
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

    private readonly ConcurrentDictionary<uint, LockableList<Player>> _playersByGuild = new ();

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
        IDropGenerator dropGenerator,
        IConfigurationChangeMediator changeMediator)
        : base(
            gameServerDefinition.GameConfiguration ?? throw new InvalidOperationException("GameServerDefinition requires a GameConfiguration"),
            persistenceContextProvider,
            mapInitializer,
            loggerFactory,
            plugInManager,
            dropGenerator,
            changeMediator)
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
    public override bool PvpEnabled => this._gameServerDefinition.PvpEnabled;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Game Server {this.Id}";
    }

    /// <inheritdoc />
    public async ValueTask ForEachGuildPlayerAsync(uint guildId, Func<Player, Task> action)
    {
        if (!this._playersByGuild.TryGetValue(guildId, out var playerList))
        {
            return;
        }

        using var readLock = await playerList.Lock.ReaderLockAsync();
        await playerList.Select(action).WhenAll().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ForEachAlliancePlayerAsync(uint guildId, Func<Player, Task> action)
    {
        if (!this._playersByGuild.TryGetValue(guildId, out var playerList))
        {
            return;
        }

        // TODO: iterate other guilds of the alliance as well; maybe introduce another dictionary with alliance players
        using var readLock = await playerList.Lock.ReaderLockAsync();
        await playerList.Select(action).WhenAll().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public override async ValueTask AddPlayerAsync(Player player)
    {
        await base.AddPlayerAsync(player).ConfigureAwait(false);
        player.PlayerLeftWorld += this.PlayerLeftWorldAsync;
        player.PlayerEnteredWorld += this.PlayerEnteredWorldAsync;
    }

    /// <inheritdoc/>
    public override async ValueTask RemovePlayerAsync(Player player)
    {
        player.PlayerEnteredWorld -= this.PlayerEnteredWorldAsync;
        player.PlayerLeftWorld -= this.PlayerLeftWorldAsync;
        await base.RemovePlayerAsync(player).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemoveGuildAsync(uint guildId)
    {
        this._playersByGuild.Remove(guildId, out _);
        this.GuildDeleted?.Invoke(this, new GuildDeletedEventArgs(guildId));
    }

    /// <inheritdoc />
    public async ValueTask RegisterGuildMemberAsync(Player guildMember)
    {
        if (guildMember.GuildStatus is null)
        {
            return;
        }

        var guildId = guildMember.GuildStatus.GuildId;
        var guildList = this._playersByGuild.GetOrAdd(guildId, id => new LockableList<Player>());
        using var writeLock = await guildList.Lock.WriterLockAsync();
        guildList.Add(guildMember);
    }

    /// <inheritdoc />
    public async ValueTask UnregisterGuildMemberAsync(Player guildMember)
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

        using var writeLock = await guildList.Lock.WriterLockAsync();
        guildList.Remove(guildMember);
    }

    private async ValueTask PlayerEnteredWorldAsync(Player player)
    {
        try
        {
            if (player is not { SelectedCharacter: { } selectedCharacter })
            {
                return;
            }

            await this.EventPublisher.PlayerEnteredGameAsync(this.Id, selectedCharacter.Id, selectedCharacter.Name).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when notifying the event publisher (player entered world).");
        }
    }

    private async ValueTask PlayerLeftWorldAsync(Player player)
    {
        if (player is not { SelectedCharacter: { } selectedCharacter })
        {
            return;
        }

        try
        {
            await this.EventPublisher.PlayerLeftGameAsync(this.Id, selectedCharacter.Id, selectedCharacter.Name, player.GuildStatus?.GuildId ?? 0).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error when notifying the event publisher (player left world).");
        }

        if (player.GuildStatus is null)
        {
            return;
        }

        await this.UnregisterGuildMemberAsync(player).ConfigureAwait(false);
        player.GuildStatus = null;
    }

    private class LockableList<T> : List<T>
    {
        public AsyncReaderWriterLock Lock { get; } = new ();
    }
}