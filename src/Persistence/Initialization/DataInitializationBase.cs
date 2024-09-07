// <copyright file="DataInitializationBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.ComponentModel.Design;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;
using MUnique.OpenMU.GameLogic.Resets;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Class to manage data initialization.
/// </summary>
public abstract class DataInitializationBase : IDataInitializationPlugIn
{
    private readonly IPersistenceContextProvider _persistenceContextProvider;

    private readonly ILoggerFactory _loggerFactory;
    private GameConfiguration? _gameConfiguration;
    private IContext? _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataInitializationBase" /> class.
    /// </summary>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    protected DataInitializationBase(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
    {
        this._persistenceContextProvider = persistenceContextProvider;
        this._loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public abstract string Key { get; }

    /// <inheritdoc />
    public abstract string Caption { get; }

    /// <summary>
    /// Gets or sets the game configuration.
    /// </summary>
    /// <exception cref="InvalidOperationException">not initialized yet.</exception>
    protected GameConfiguration GameConfiguration
    {
        get => this._gameConfiguration ?? throw new InvalidOperationException("not initialized yet.");
        set => this._gameConfiguration = value;
    }

    /// <summary>
    /// Gets or sets the context.
    /// </summary>
    /// <exception cref="InvalidOperationException">not initialized yet.</exception>
    protected IContext Context
    {
        get => this._context ?? throw new InvalidOperationException("not initialized yet.");
        set => this._context = value;
    }

    /// <summary>
    /// Gets the game configuration initializer.
    /// </summary>
    protected abstract IInitializer GameConfigurationInitializer { get; }

    /// <summary>
    /// Gets the maps initializer.
    /// </summary>
    protected abstract IGameMapsInitializer GameMapsInitializer { get; }

    /// <summary>
    /// Gets the test accounts initializer.
    /// </summary>
    protected abstract IInitializer? TestAccountsInitializer { get; }

    /// <summary>
    /// Creates the initial data for a server.
    /// </summary>
    /// <param name="numberOfGameServers">The number of game servers.</param>
    /// <param name="createTestAccounts">If set to <c>true</c>, test accounts should be created.</param>
    public async Task CreateInitialDataAsync(byte numberOfGameServers, bool createTestAccounts)
    {
        BaseMapInitializer.ClearDefaultDropItemGroups();
        using (var temporaryContext = this._persistenceContextProvider.CreateNewContext())
        {
            this.GameConfiguration = temporaryContext.CreateNew<GameConfiguration>();
            this.GameConfiguration.SetGuid(1);
            this.CreateSystemConfiguration(temporaryContext);
            using var tempSuspension = temporaryContext.SuspendChangeNotifications();
            await temporaryContext.SaveChangesAsync().ConfigureAwait(false);
        }

        using var contextWithConfiguration = this._persistenceContextProvider.CreateNewContext(this.GameConfiguration);
        using var notificationSuspension = contextWithConfiguration.SuspendChangeNotifications();
        this.Context = contextWithConfiguration;
        this.CreateGameClientDefinition();
        await this.CreateChatServerDefinitionAsync().ConfigureAwait(false);
        this.GameConfigurationInitializer.Initialize();

        var gameServerConfiguration = this.CreateGameServerConfiguration(this.GameConfiguration.Maps);
        await this.CreateGameServerDefinitionsAsync(gameServerConfiguration, numberOfGameServers).ConfigureAwait(false);
        await this.CreateConnectServerDefinitionAsync().ConfigureAwait(false);
        await this.Context.SaveChangesAsync().ConfigureAwait(false);

        this.GameMapsInitializer.SetSafezoneMaps();

        if (createTestAccounts)
        {
            this.TestAccountsInitializer?.Initialize();
        }

        if (!AppDomain.CurrentDomain.GetAssemblies().Contains(typeof(GameServer.GameServer).Assembly))
        {
            // should never happen, but the access to the GameServer type is a trick to load the assembly into the current domain.
        }

        var dataSource = new GameConfigurationDataSource(this._loggerFactory.CreateLogger<GameConfigurationDataSource>(), this._persistenceContextProvider);
        await dataSource.GetOwnerAsync(this.GameConfiguration.GetId());
        var referenceHandler = new ByDataSourceReferenceHandler(dataSource);
        var serviceContainer = new ServiceContainer();
        serviceContainer.AddService(typeof(IPersistenceContextProvider), this._persistenceContextProvider);
        var plugInManager = new PlugInManager(null, this._loggerFactory, serviceContainer, referenceHandler);
        plugInManager.DiscoverAndRegisterPlugIns();
        plugInManager.KnownPlugInTypes.ForEach(plugInType =>
        {
            var plugInConfiguration = this.Context.CreateNew<PlugInConfiguration>();
            plugInConfiguration.SetGuid(plugInType.GUID);
            plugInConfiguration.TypeId = plugInType.GUID;
            plugInConfiguration.IsActive = true;
            this.GameConfiguration.PlugInConfigurations.Add(plugInConfiguration);

            if (plugInType.GetInterfaces().Contains(typeof(ISupportDefaultCustomConfiguration)))
            {
                this.CreateDefaultPlugInConfiguration(plugInType, plugInConfiguration, referenceHandler);
            }

            if (plugInType == typeof(BlessJewelConsumeHandlerPlugIn))
            {
                var config = new BlessJewelConsumeHandlerPlugInConfiguration
                {
                    MaximumLevel = 5,
                    MinimumLevel = 0,
                    SuccessRatePercentage = 100,
                    SuccessRateBonusWithLuckPercentage = 0,
                    ResetToLevel0WhenFailMinLevel = 0,
                };

                if (this.GameConfiguration.Items.FirstOrDefault(item => item is { Group: 13, Number: 37 }) is { } fenrir)
                {
                    config.RepairTargetItems.Add(fenrir);
                }

                plugInConfiguration.SetConfiguration(config, referenceHandler);
            }

            // We don't move the player anymore by his request. This was usually requested after a player performed a skill.
            // However, it adds way for cheaters to move through the map.
            // The plugin is therefore deactivated by default.
            if (plugInType.IsAssignableTo(typeof(CharacterMoveBaseHandlerPlugIn)))
            {
                plugInConfiguration.IsActive = false;
            }

            // Resets are disabled by default.
            if (plugInType == typeof(ResetFeaturePlugIn))
            {
                plugInConfiguration.IsActive = false;
            }
        });

        this.AddAllUpdateEntries(plugInManager);

        await this.Context.SaveChangesAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the game client definition.
    /// </summary>
    protected abstract void CreateGameClientDefinition();

    private void CreateDefaultPlugInConfiguration(Type plugInType, PlugInConfiguration plugInConfiguration, ReferenceHandler referenceHandler)
    {
        try
        {
            var plugin = (ISupportDefaultCustomConfiguration)Activator.CreateInstance(plugInType)!;
            var defaultConfig = plugin.CreateDefaultConfig();
            plugInConfiguration.SetConfiguration(defaultConfig, referenceHandler);
        }
        catch (Exception ex)
        {
            this._loggerFactory.CreateLogger(this.GetType()).LogWarning(ex, "Could not create custom default configuration for plugin type {plugInType}", plugInType);
        }
    }

    private void AddAllUpdateEntries(PlugInManager plugInManager)
    {
        var updates = plugInManager.GetStrategyProvider<int, IConfigurationUpdatePlugIn>()
                          ?.AvailableStrategies.Where(up => up.DataInitializationKey == this.Key)
                          .OrderBy(up => up.Version)
                          .ToList();
        if (updates is not { Count: > 0 })
        {
            return;
        }

        foreach (var update in updates)
        {
            var entry = this.Context.CreateNew<ConfigurationUpdate>();
            entry.Version = (int)update.Version;
            entry.Name = update.Name;
            entry.Description = update.Description;
            entry.CreatedAt = update.CreatedAt;
            entry.InstalledAt = DateTime.UtcNow;
        }

        var updateState = this.Context.CreateNew<ConfigurationUpdateState>();
        updateState.InitializationKey = this.Key;
        updateState.CurrentInstalledVersion = updates.Max(u => (int)u.Version);
    }

    private async ValueTask CreateConnectServerDefinitionAsync()
    {
        var client = (await this.Context.GetAsync<GameClientDefinition>().ConfigureAwait(false)).First();
        var connectServer = this.Context.CreateNew<ConnectServerDefinition>();
        connectServer.SetGuid(1);
        connectServer.Client = client;
        connectServer.ClientListenerPort = 44405;
        connectServer.Description = $"Connect Server ({new ClientVersion(client.Season, client.Episode, client.Language)})";
        connectServer.DisconnectOnUnknownPacket = true;
        connectServer.MaximumReceiveSize = 6;
        connectServer.Timeout = new TimeSpan(0, 1, 0);
        connectServer.CurrentPatchVersion = new byte[] { 1, 3, 0x2B };
        connectServer.PatchAddress = "patch.muonline.webzen.com";
        connectServer.MaxConnectionsPerAddress = 30;
        connectServer.CheckMaxConnectionsPerAddress = true;
        connectServer.MaxConnections = 10000;
        connectServer.ListenerBacklog = 100;
        connectServer.MaxFtpRequests = 1;
        connectServer.MaxIpRequests = 5;
        connectServer.MaxServerListRequests = 20;
    }

    private async ValueTask CreateGameServerDefinitionsAsync(GameServerConfiguration gameServerConfiguration, int numberOfServers)
    {
        for (int i = 0; i < numberOfServers; i++)
        {
            var server = this.Context!.CreateNew<GameServerDefinition>();
            server.SetGuid((short)i);
            server.ServerID = (byte)i;
            server.Description = $"Server {i}";
            server.ExperienceRate = 1.0f;
            server.GameConfiguration = this.GameConfiguration;
            server.ServerConfiguration = gameServerConfiguration;

            foreach (var client in await this.Context.GetAsync<GameClientDefinition>().ConfigureAwait(false))
            {
                var endPoint = this.Context.CreateNew<GameServerEndpoint>();
                endPoint.SetGuid((short)i, (short)server.Endpoints.Count);
                endPoint.Client = client;
                endPoint.NetworkPort = 55901 + i;
                server.Endpoints.Add(endPoint);
            }
        }
    }

    private async ValueTask CreateChatServerDefinitionAsync()
    {
        var server = this.Context!.CreateNew<ChatServerDefinition>();
        server.SetGuid(0);
        server.ServerId = 0;
        server.Description = "Chat Server";

        var client = (await this.Context!.GetAsync<GameClientDefinition>().ConfigureAwait(false)).First();
        var endPoint = this.Context.CreateNew<ChatServerEndpoint>();
        server.SetGuid(0);
        endPoint.Client = client;
        endPoint.NetworkPort = 55980;
        server.Endpoints.Add(endPoint);
    }

    private GameServerConfiguration CreateGameServerConfiguration(ICollection<GameMapDefinition> maps)
    {
        var gameServerConfiguration = this.Context.CreateNew<GameServerConfiguration>();
        gameServerConfiguration.SetGuid(0);
        gameServerConfiguration.MaximumPlayers = 1000;

        // by default we add every map to a server configuration
        foreach (var map in maps)
        {
            gameServerConfiguration.Maps.Add(map);
        }

        return gameServerConfiguration;
    }

    private void CreateSystemConfiguration(IContext context)
    {
        var systemConfiguration = context.CreateNew<SystemConfiguration>();
        systemConfiguration.SetGuid(0);
        systemConfiguration.AutoStart = true;
        systemConfiguration.AutoUpdateSchema = true;
        systemConfiguration.ReadConsoleInput = false;

        var (type, param) = IpAddressResolverFactory.DetermineBestFittingResolver(Environment.GetCommandLineArgs());
        systemConfiguration.IpResolver = type;
        systemConfiguration.IpResolverParameter = param;
    }
}