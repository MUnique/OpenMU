// <copyright file="DataInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Resets;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Persistence.Initialization.Maps;
    using MUnique.OpenMU.Persistence.Initialization.TestAccounts;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Class to manage data initialization.
    /// </summary>
    public class DataInitialization
    {
        private readonly IPersistenceContextProvider persistenceContextProvider;
        private readonly ILoggerFactory loggerFactory;
        private GameConfiguration? gameConfiguration;
        private IContext? context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataInitialization" /> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public DataInitialization(IPersistenceContextProvider persistenceContextProvider, ILoggerFactory loggerFactory)
        {
            this.persistenceContextProvider = persistenceContextProvider;
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Creates the initial data for a server.
        /// </summary>
        public void CreateInitialData()
        {
            BaseMapInitializer.ClearDefaultDropItemGroups();
            using (var temporaryContext = this.persistenceContextProvider.CreateNewContext())
            {
                this.gameConfiguration = temporaryContext.CreateNew<GameConfiguration>();
                temporaryContext.SaveChanges();
            }

            using var contextWithConfiguration = this.persistenceContextProvider.CreateNewContext(this.gameConfiguration);
            this.context = contextWithConfiguration;
            this.CreateGameClientDefinitions();
            this.CreateChatServerDefinition();
            new GameConfigurationInitializer(this.context, this.gameConfiguration).Initialize();

            var gameServerConfiguration = this.CreateGameServerConfiguration(this.gameConfiguration.Maps);
            this.CreateGameServerDefinitions(gameServerConfiguration, 3);
            this.CreateConnectServerDefinitions();
            this.context.SaveChanges();

            new MapsInitializer(this.context, this.gameConfiguration).SetSafezoneMaps();

            new TestAccountsInitialization(this.context, this.gameConfiguration).Initialize();

            if (!AppDomain.CurrentDomain.GetAssemblies().Contains(typeof(GameServer).Assembly))
            {
                // should never happen, but the access to the GameServer type is a trick to load the assembly into the current domain.
            }

            var plugInManager = new PlugInManager(null, this.loggerFactory, null);
            plugInManager.DiscoverAndRegisterPlugIns();
            plugInManager.KnownPlugInTypes.ForEach(plugInType =>
            {
                var plugInConfiguration = this.context.CreateNew<PlugInConfiguration>();
                plugInConfiguration.TypeId = plugInType.GUID;
                plugInConfiguration.IsActive = true;
                this.gameConfiguration.PlugInConfigurations.Add(plugInConfiguration);

                // Resets are disabled by default.
                if (plugInType == typeof(ResetFeaturePlugIn))
                {
                    plugInConfiguration.IsActive = false;
                    plugInConfiguration.SetConfiguration(new ResetConfiguration());
                }
            });

            this.context.SaveChanges();
        }

        private void CreateGameClientDefinitions()
        {
            var clientDefinition = this.context!.CreateNew<GameClientDefinition>();
            clientDefinition.Season = 6;
            clientDefinition.Episode = 3;
            clientDefinition.Language = ClientLanguage.English;
            clientDefinition.Version = new byte[] { 0x31, 0x30, 0x34, 0x30, 0x34 };
            clientDefinition.Serial = Encoding.ASCII.GetBytes("k1Pk2jcET48mxL3b");
            clientDefinition.Description = "Season 6 Episode 3 GMO Client";

            var version075Definition = this.context.CreateNew<GameClientDefinition>();
            version075Definition.Season = 0;
            version075Definition.Episode = 75;
            version075Definition.Language = ClientLanguage.Invariant; // it doesn't fit into any available category - maybe it's so old that it didn't have differences in the protocol yet.
            version075Definition.Version = new byte[] { 0x30, 0x37, 0x35, 0x30, 0x30 }; // the last two bytes are not relevant as te 0.75 does only use the first 3 bytes.
            version075Definition.Serial = Encoding.ASCII.GetBytes("sudv(*40ds7lkN2n");
            version075Definition.Description = "Version 0.75 Client";
        }

        private void CreateConnectServerDefinitions()
        {
            var i = 0;
            foreach (var client in this.context!.Get<GameClientDefinition>().ToList())
            {
                var connectServer = this.context.CreateNew<ConnectServerDefinition>();
                connectServer.ServerId = (byte)i;
                connectServer.Client = client;
                connectServer.ClientListenerPort = 44405 + i;
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
                i++;
            }
        }

        private void CreateGameServerDefinitions(GameServerConfiguration gameServerConfiguration, int numberOfServers)
        {
            for (int i = 0; i < numberOfServers; i++)
            {
                var server = this.context!.CreateNew<GameServerDefinition>();
                server.ServerID = (byte)i;
                server.Description = $"Server {i}";
                server.ExperienceRate = 1.0f;
                server.GameConfiguration = this.gameConfiguration;
                server.ServerConfiguration = gameServerConfiguration;

                var j = 0;
                foreach (var client in this.context.Get<GameClientDefinition>().ToList())
                {
                    var endPoint = this.context.CreateNew<GameServerEndpoint>();
                    endPoint.Client = client;
                    endPoint.NetworkPort = 55901 + i + j;
                    server.Endpoints.Add(endPoint);
                    j += 20;
                }
            }
        }

        private void CreateChatServerDefinition()
        {
            var server = this.context!.CreateNew<ChatServerDefinition>();
            server.ServerId = 0;
            server.Description = "Chat Server";

            var i = 0;
            foreach (var client in this.context.Get<GameClientDefinition>()
                .OrderByDescending(c => c.Season) // Season 6 should get the standard port
                .ToList())
            {
                var endPoint = this.context.CreateNew<ChatServerEndpoint>();
                endPoint.Client = client;
                endPoint.NetworkPort = 55980 + i++;
                server.Endpoints.Add(endPoint);
            }
        }

        private GameServerConfiguration CreateGameServerConfiguration(ICollection<GameMapDefinition> maps)
        {
            var gameServerConfiguration = this.context!.CreateNew<GameServerConfiguration>();
            gameServerConfiguration.MaximumPlayers = 1000;

            // by default we add every map to a server configuration
            foreach (var map in maps)
            {
                gameServerConfiguration.Maps.Add(map);
            }

            return gameServerConfiguration;
        }
    }
}
