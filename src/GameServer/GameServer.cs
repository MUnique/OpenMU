// <copyright file="GameServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Messenger;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// The game server to which game clients can connect.
/// </summary>
public sealed class GameServer : IGameServer, IDisposable, IGameServerContextProvider
{
    private readonly ILogger<GameServer> _logger;

    private readonly IGameServerContext _gameContext;

    private readonly ICollection<IGameServerListener> _listeners = new List<IGameServerListener>();

    private ServerState _serverState;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServer" /> class.
    /// </summary>
    /// <param name="gameServerDefinition">The game server definition.</param>
    /// <param name="guildServer">The guild server.</param>
    /// <param name="eventPublisher">The message publisher.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="friendServer">The friend server.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="changeMediator"> The change mediatior.</param>
    public GameServer(
        GameServerDefinition gameServerDefinition,
        IGuildServer guildServer,
        IEventPublisher eventPublisher,
        ILoginServer loginServer,
        IPersistenceContextProvider persistenceContextProvider,
        IFriendServer friendServer,
        ILoggerFactory loggerFactory,
        PlugInManager plugInManager,
        IConfigurationChangeMediator changeMediator)
    {
        this.Id = gameServerDefinition.ServerID;
        this.Description = gameServerDefinition.Description;
        this.ConfigurationId = gameServerDefinition.GetId();
        this._logger = loggerFactory.CreateLogger<GameServer>();
        try
        {
            var gameConfiguration = gameServerDefinition.GameConfiguration ?? throw Error.NotInitializedProperty(gameServerDefinition, nameof(gameServerDefinition.GameConfiguration));
            var dropGenerator = new DefaultDropGenerator(gameConfiguration, Rand.GetRandomizer());
            var mapInitializer = new GameServerMapInitializer(gameServerDefinition, loggerFactory.CreateLogger<GameServerMapInitializer>(), dropGenerator, changeMediator);
            this._gameContext = new GameServerContext(gameServerDefinition, guildServer, eventPublisher, loginServer, friendServer, persistenceContextProvider, mapInitializer, loggerFactory, plugInManager, dropGenerator, changeMediator);
            this._gameContext.GameMapCreated += (_, _) => this.OnPropertyChanged(nameof(this.Context));
            this._gameContext.GameMapRemoved += (_, _) => this.OnPropertyChanged(nameof(this.Context));
            mapInitializer.PlugInManager = this._gameContext.PlugInManager;
            mapInitializer.PathFinderPool = this._gameContext.PathFinderPool;
        }
        catch (Exception ex)
        {
            this._logger.LogCritical(ex, "Error during map initialization");
            throw;
        }
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the identifier of the server.
    /// </summary>
    public byte Id { get; }

    /// <inheritdoc />
    public Guid ConfigurationId { get; }

    /// <inheritdoc/>
    public string Description { get; }

    /// <summary>
    /// Gets the server context.
    /// </summary>
    public IGameServerContext Context => this._gameContext;

    /// <inheritdoc/>
    public ServerState ServerState
    {
        get => this._serverState;
        set
        {
            if (this._serverState != value)
            {
                this._serverState = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public ServerType Type => ServerType.GameServer;

    /// <inheritdoc/>
    public int MaximumConnections => this.Context.ServerConfiguration.MaximumPlayers;

    /// <inheritdoc/>
    public int CurrentConnections => this.Context.PlayerCount;

    /// <inheritdoc/>
    int IManageableServer.Id => this.Id;

    /// <summary>
    /// Adds the listener.
    /// </summary>
    /// <param name="listener">The listener.</param>
    public void AddListener(IGameServerListener listener)
    {
        this._listeners.Add(listener);
        listener.PlayerConnected += this.OnPlayerConnectedAsync;
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (this.ServerState == ServerState.Stopped)
        {
            await this.StartAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask StartAsync()
    {
        using var logScope = this._logger.BeginScope(("GameServer", this.Id));
        this.ServerState = ServerState.Starting;
        try
        {
            foreach (var listener in this._listeners)
            {
                await listener.StartAsync().ConfigureAwait(false);
            }

            this.ServerState = ServerState.Started;
        }
        catch (Exception e)
        {
            this._logger.LogError(e, $"Could not start the server listeners: {e.Message}");
            foreach (var listener in this._listeners)
            {
                listener.Stop();
            }

            this.ServerState = ServerState.Stopped;
        }
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await this.ShutdownAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShutdownAsync()
    {
        using var logScope = this._logger.BeginScope(("GameServer", this.Id));
        this.ServerState = ServerState.Stopping;
        this._logger.LogInformation("Server is shutting down. Stopping listener...");
        foreach (var listener in this._listeners)
        {
            listener.Stop();
        }

        this._logger.LogInformation("Saving all open sessions...");

        // Because disconnecting might directly change the internal player list, we first collect all players.
        var playerList = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        await playerList.Select(player => player.DisconnectAsync().AsTask()).WhenAll().ConfigureAwait(false);

        this.ServerState = ServerState.Stopped;
        this._logger.LogInformation("Server shutted down.");
    }

    /// <inheritdoc/>
    public async ValueTask GuildChatMessageAsync(uint guildId, string sender, string message)
    {
        string messageSend = message;

        if (!messageSend.StartsWith("@", StringComparison.InvariantCulture))
        {
            messageSend = "@" + message;
        }

        await this._gameContext.ForEachGuildPlayerAsync(guildId, player => player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(messageSend, sender, ChatMessageType.Guild)).AsTask()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask AllianceChatMessageAsync(uint guildId, string sender, string message)
    {
        string messageSend = message;

        if (!messageSend.StartsWith("@@", StringComparison.InvariantCulture))
        {
            messageSend = "@@" + message;
        }

        await this._gameContext.ForEachAlliancePlayerAsync(guildId, player => player.InvokeViewPlugInAsync<IChatViewPlugIn>(p => p.ChatMessageAsync(messageSend, sender, ChatMessageType.Alliance)).AsTask()).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask LetterReceivedAsync(LetterHeader letter)
    {
        if (letter.ReceiverName is null)
        {
            throw new InvalidOperationException("Letter ReceiverName must be provided.");
        }

        if (this._gameContext.GetPlayerByCharacterName(letter.ReceiverName) is not { } player)
        {
            return;
        }

        if (player.SelectedCharacter?.Letters.Contains(letter) ?? true)
        {
            // We already know this letter, so we do nothing.
            // This usually never happens, but who knows...
            return;
        }

        // The letter we get here might not be of the right type to be attached
        // to the players context and letter collection.
        // So, we simply load it from the database.
        var loadedLetter = await player.PersistenceContext.GetByIdAsync<LetterHeader>(letter.Id).ConfigureAwait(false);
        if (loadedLetter is null)
        {
            // something went wrong here, this should never happen.
            this._logger.LogError("Couldn't find letter with id {0} on the database: {1}", letter.Id, letter);
            return;
        }

        var letterIndex = player.SelectedCharacter.Letters.IndexOf(loadedLetter);
        if (letterIndex < 0)
        {
            letterIndex = player.SelectedCharacter!.Letters.Count;
            player.SelectedCharacter.Letters.Add(loadedLetter);
        }

        await player.InvokeViewPlugInAsync<IAddToLetterListPlugIn>(p => p.AddToLetterListAsync(letter, (ushort)letterIndex, true)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<bool> DisconnectPlayerAsync(string playerName)
    {
        var player = this._gameContext.GetPlayerByCharacterName(playerName);
        if (player != null)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You got disconnected by a game master.", MessageType.BlueNormal)).ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public async ValueTask<bool> DisconnectAccountAsync(string accountName)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        var player = players.FirstOrDefault(p => p.Account?.LoginName == accountName);
        if (player != null)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You got disconnected by an administrator.", MessageType.BlueNormal)).ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public async ValueTask<bool> BanPlayerAsync(string playerName)
    {
        var player = this._gameContext.GetPlayerByCharacterName(playerName);
        if (player?.Account is not null)
        {
            player.Account.State = AccountState.TemporarilyBanned;
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("Your account has been temporarily banned by a game master.", MessageType.BlueNormal)).ConfigureAwait(false);
            await player.DisconnectAsync().ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public async ValueTask AssignGuildToPlayerAsync(string characterName, GuildMemberStatus guildStatus)
    {
        var player = this._gameContext.GetPlayerByCharacterName(characterName);
        if (player is null)
        {
            return;
        }

        player.GuildStatus = guildStatus;
        await player.ForEachWorldObserverAsync<IAssignPlayersToGuildPlugIn>(p => p.AssignPlayerToGuildAsync(player, true), true).ConfigureAwait(false);
        await this.Context.RegisterGuildMemberAsync(player).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayerAlreadyLoggedInAsync(byte serverId, string loginName)
    {
        var players = await this._gameContext.GetPlayersAsync().ConfigureAwait(false);
        var affectedPlayer = players.FirstOrDefault(p => p.Account?.LoginName == loginName);

        if (affectedPlayer is not null)
        {
            await affectedPlayer.ShowMessageAsync("Another user attempted to login this account. If it wasn't you, we suggest you to change your password.").ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public ValueTask SendGlobalMessageAsync(string message, MessageType messageType)
    {
        return this._gameContext.SendGlobalMessageAsync(message, messageType);
    }

    /// <inheritdoc/>
    public async ValueTask FriendRequestAsync(string requester, string receiver)
    {
        if (this._gameContext.GetPlayerByCharacterName(receiver) is { } player)
        {
            await player.InvokeViewPlugInAsync<IShowFriendRequestPlugIn>(p => p.ShowFriendRequestAsync(requester)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask FriendOnlineStateChangedAsync(string player, string friend, int serverId)
    {
        if (this._gameContext.GetPlayerByCharacterName(player) is { } observerPlayer)
        {
            await observerPlayer.InvokeViewPlugInAsync<IFriendStateUpdatePlugIn>(p => p.FriendStateUpdateAsync(friend, serverId)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask InitializeMessengerAsync(MessengerInitializationData initializationData)
    {
        if (this._gameContext.GetPlayerByCharacterName(initializationData.PlayerName) is { } player)
        {
            await player.InvokeViewPlugInAsync<IInitializeMessengerPlugIn>(p => p.InitializeMessengerAsync(initializationData, this.Context.Configuration.MaximumLetters)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask ChatRoomCreatedAsync(ChatServerAuthenticationInfo authenticationInfo, string creatorName)
    {
        if (this._gameContext.GetPlayerByCharacterName(authenticationInfo.ClientName) is { } player)
        {
            await player.InvokeViewPlugInAsync<IChatRoomCreatedPlugIn>(p => p.ChatRoomCreatedAsync(authenticationInfo, creatorName, true)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask GuildDeletedAsync(uint guildId)
    {
        await this._gameContext.RemoveGuildAsync(guildId).ConfigureAwait(false);
        await this._gameContext.ForEachGuildPlayerAsync(guildId, p => this.RemovePlayerFromGuildAsync(p, false).AsTask()).ConfigureAwait(false);

        //// todo: alliance things?
    }

    /// <inheritdoc/>
    public async ValueTask GuildPlayerKickedAsync(string playerName)
    {
        var player = this._gameContext.GetPlayerByCharacterName(playerName);
        if (player is null)
        {
            return;
        }

        await this.RemovePlayerFromGuildAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates an instance of <see cref="ServerInfo"/> with the data of this instance.
    /// </summary>
    /// <returns>The created <see cref="ServerInfo"/>.</returns>
    public ServerInfo CreateServerInfo() => new(this.Id, this.Description, this.CurrentConnections, this.MaximumConnections);

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.Id} - [{this.Description}]";
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "gameContext", Justification = "Null-conditional confuses the code analysis.")]
    public void Dispose()
    {
        (this._gameContext as IDisposable)?.Dispose();
    }

    private async ValueTask RemovePlayerFromGuildAsync(Player player, bool unregisterFromContext = true)
    {
        if (unregisterFromContext && player.GuildStatus?.GuildId is { } guildId)
        {
            await this._gameContext.UnregisterGuildMemberAsync(player).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IPlayerLeftGuildPlugIn>(p => p.PlayerLeftGuildAsync(player), true).ConfigureAwait(false);
        player.GuildStatus = null;
        await player.InvokeViewPlugInAsync<IGuildKickResultPlugIn>(p => p.GuildKickResultAsync(GuildKickSuccess.KickSucceeded)).ConfigureAwait(false);
    }

    private async ValueTask OnPlayerConnectedAsync(PlayerConnectedEventArgs e)
    {
        var player = e.ConntectedPlayer;
        await this._gameContext.AddPlayerAsync(player).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowLoginWindowPlugIn>(p => p.ShowLoginWindowAsync()).ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        e.ConntectedPlayer.PlayerDisconnected += this.OnPlayerDisconnectedAsync;
        this.OnPropertyChanged(nameof(this.CurrentConnections));
    }

    private async ValueTask OnPlayerDisconnectedAsync(Player remotePlayer)
    {
        if (!remotePlayer.IsTemplatePlayer)
        {
            await this.SaveSessionOfPlayerAsync(remotePlayer).ConfigureAwait(false);
            await this.SetOfflineAtLoginServerAsync(remotePlayer).ConfigureAwait(false);
        }

        await remotePlayer.DisposeAsync().ConfigureAwait(false);
        this.OnPropertyChanged(nameof(this.CurrentConnections));
    }

    private async ValueTask SetOfflineAtLoginServerAsync(Player player)
    {
        try
        {
            if (player.Account?.LoginName is { } loginName)
            {
                await this.Context.LoginServer.LogOffAsync(loginName, this.Id).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError($"Couldn't set offline state at login server. Player: {this}", ex);
        }
    }

    private async ValueTask SaveSessionOfPlayerAsync(Player player)
    {
        try
        {
            // Recover items placed in an NPC or trade dialog when player is disconnected
            if (player.BackupInventory is not null)
            {
                player.Inventory!.Clear();
                player.BackupInventory.RestoreItemStates();
                foreach (var item in player.BackupInventory.Items)
                {
                    await player.Inventory.AddItemAsync(item.ItemSlot, item).ConfigureAwait(false);
                }

                player.Inventory.ItemStorage.Money = player.BackupInventory.Money;
            }
            else if (player is { TemporaryStorage: { } storage, Inventory: { } inventory })
            {
                if (!await inventory.TryTakeAllAsync(storage).ConfigureAwait(false))
                {
                    // This should never happen since the space is checked before mixing
                    this._logger.LogWarning($"Could not take fit items of player {player}");
                }
            }
            else
            {
                // nothing else to restore.
            }

            if (!await player.SaveProgressAsync().ConfigureAwait(false))
            {
                this._logger.LogWarning($"Could not save session of player {player}");
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't Save at Disconnect. Player: {player}", this);

            // TODO: Log Character/Account values, to be able to restore players data if necessary.
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}