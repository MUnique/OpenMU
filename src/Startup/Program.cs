// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using log4net.Config;
    using MUnique.OpenMU.AdminPanel;
    using MUnique.OpenMU.ChatServer;
    using MUnique.OpenMU.ConnectServer;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.FriendServer;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.GuildServer;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.LoginServer;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.EntityFramework;
    using MUnique.OpenMU.Persistence.Initialization;
    using MUnique.OpenMU.Persistence.InMemory;

    /// <summary>
    /// The startup class for an all-in-one game server.
    /// </summary>
    internal sealed class Program : IDisposable
    {
        private static readonly string Log4NetConfigFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + typeof(Program).GetTypeInfo().Namespace + ".exe.log4net.xml";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private readonly AdminPanel adminPanel;
        private readonly IDictionary<int, IGameServer> gameServers = new Dictionary<int, IGameServer>();
        private readonly IList<IManageableServer> servers = new List<IManageableServer>();
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// Constructor for the main entry program.
        /// </summary>
        /// <param name="args">The command line args.</param>
        public Program(string[] args)
        {
            if (args.Contains("-demo"))
            {
                this.persistenceContextProvider = new InMemoryPersistenceContextProvider();
                var initialization = new DataInitialization(this.persistenceContextProvider);
                initialization.CreateInitialData();
            }
            else
            {
                this.persistenceContextProvider = this.PrepareRepositoryManager(args.Contains("-reinit"));
            }

            Log.Info("Start initializing sub-components");
            var signalRServerObserver = new SignalRGameServerStateObserver();
            var persistenceContext = this.persistenceContextProvider.CreateNewConfigurationContext();
            var loginServer = new LoginServer();
            var chatServer = new ChatServerListener(55980, signalRServerObserver);
            this.servers.Add(chatServer);
            var guildServer = new GuildServer(this.gameServers, this.persistenceContextProvider);
            var friendServer = new FriendServer(this.gameServers, chatServer, this.persistenceContextProvider);
            var connectServer = ConnectServerFactory.CreateConnectServer(signalRServerObserver);
            this.servers.Add(connectServer);
            Log.Info("Start initializing game servers");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            foreach (var gameServerDefinition in persistenceContext.Get<GameServerDefinition>())
            {
                using (ThreadContext.Stacks["gameserver"].Push(gameServerDefinition.ServerID.ToString()))
                {
                    var gameServer = new GameServer(gameServerDefinition, guildServer, loginServer, this.persistenceContextProvider, friendServer, signalRServerObserver);
                    foreach (var mainPacketHandler in gameServer.Context.PacketHandlers.Take(1))
                    {
                        // At the moment only one main packet handler should be used.
                        // A TCP port can only be used for one TCP listener, so we have to introduce something to pair ports with main packets handlers.
                        gameServer.AddListener(new DefaultTcpGameServerListener(gameServerDefinition.NetworkPort, gameServer.ServerInfo, gameServer.Context, connectServer, mainPacketHandler));
                    }

                    this.servers.Add(gameServer);
                    this.gameServers.Add(gameServer.Id, gameServer);
                    Log.InfoFormat("Game Server {0} - [{1}] initialized", gameServer.Id, gameServer.Description);
                }
            }

            stopwatch.Stop();
            Log.Info($"All game servers initialized, elapsed time: {stopwatch.Elapsed}");
            Log.Info("Start initializing admin panel");

            this.adminPanel = new AdminPanel(1234, this.servers, this.persistenceContextProvider);
            Log.Info("Admin panel initialized");

            if (args.Contains("-autostart"))
            {
                chatServer.Start();
                foreach (var gameServer in this.gameServers.Values)
                {
                    gameServer.Start();
                }

                connectServer.Start();
            }
        }

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The command line args.</param>
        public static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(Log4NetConfigFilePath));

            using (new Program(args))
            {
                bool exit = false;
                while (!exit)
                {
                    switch (Console.ReadLine()?.ToLower())
                    {
                        case "exit":
                            exit = true;
                            break;
                        case "gc":
                            GC.Collect();
                            break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var server in this.servers)
            {
                Log.InfoFormat("Shutting down server {0}", server.Id);
                server.Shutdown();
                (server as IDisposable)?.Dispose();
            }

            this.adminPanel.Dispose();
            (this.persistenceContextProvider as IDisposable)?.Dispose();
        }

        private IPersistenceContextProvider PrepareRepositoryManager(bool reinit)
        {
            PersistenceContextProvider.InitializeSqlLogging();
            var manager = new PersistenceContextProvider();
            if (reinit || !manager.DatabaseExists())
            {
                Log.Info("The database is getting (re-)ininitialized...");
                manager.ReCreateDatabase();
                var initialization = new DataInitialization(manager);
                initialization.CreateInitialData();
                Log.Info("...initialization finished.");
            }
            else if (!manager.IsDatabaseUpToDate())
            {
                Console.WriteLine("The database needs to be updated before the server can be started. Apply update? (y/n)");
                var key = Console.ReadLine()?.ToLowerInvariant();
                if (key == "y")
                {
                    manager.ApplyAllPendingUpdates();
                    Console.WriteLine("The database has been successfully updated.");
                }
                else
                {
                    Console.WriteLine("Cancelled the update process, can't start the server.");
                    return null;
                }
            }

            return manager;
        }
    }
}