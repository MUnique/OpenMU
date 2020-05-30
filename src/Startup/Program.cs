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
    using System.Threading;
    using System.Threading.Tasks;
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
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.EntityFramework;
    using MUnique.OpenMU.Persistence.Initialization;
    using MUnique.OpenMU.Persistence.InMemory;
    using MUnique.OpenMU.PublicApi;

    /// <summary>
    /// The startup class for an all-in-one game server.
    /// </summary>
    internal sealed class Program : IDisposable
    {
        private static readonly string Log4NetConfigFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + typeof(Program).GetTypeInfo().Namespace + ".exe.log4net.xml";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private readonly AdminPanel adminPanel;
        private readonly ApiHost apiHost;
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
            this.persistenceContextProvider = this.DeterminePersistenceContextProvider(args);

            var ipResolver = IpAddressResolverFactory.DetermineIpResolver(args);

            Log.Info("Start initializing sub-components");
            var serverConfigListener = new ServerConfigurationChangeListener(this.servers);
            var persistenceContext = this.persistenceContextProvider.CreateNewConfigurationContext();

            this.InitializeGameServers(persistenceContext, ipResolver);

            Log.Info("Start API...");
            this.apiHost = new ApiHost(this.gameServers.Values, this.servers.OfType<IConnectServer>(), Log4NetConfigFilePath);
            this.apiHost.Start();
            Log.Info("Started API");

            var adminPort = this.DetermineAdminPort(args);
            Log.Info($"Start initializing admin panel for port {adminPort}.");
            this.adminPanel = new AdminPanel(adminPort, this.servers, this.persistenceContextProvider, serverConfigListener, Log4NetConfigFilePath);
            this.adminPanel.Start();
            Log.Info($"Admin panel initialized, port {adminPort}.");

            if (args.Contains("-autostart"))
            {
                foreach (var chatServer in this.servers.OfType<ChatServer>())
                {
                    chatServer.Start();
                }

                foreach (var gameServer in this.gameServers.Values)
                {
                    gameServer.Start();
                }

                foreach (var connectServer in this.servers.OfType<IConnectServer>())
                {
                    connectServer.Start();
                }
            }
        }

        /// <summary>
        /// The main method.
        /// </summary>
        /// <param name="args">The command line args.</param>
        public static async Task Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.ConfigureAndWatch(logRepository, new FileInfo(Log4NetConfigFilePath));
            using var exitCts = new CancellationTokenSource();
            var exitToken = exitCts.Token;
            var confirmExit = false;
            var isDaemonMode = args.Contains("-daemon");

            void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
            {
                if (confirmExit) {
                    exitCts.Cancel();
                    Console.CancelKeyPress -= OnCancelKeyPress;
                    Console.WriteLine("\nBye! Press enter to finish");
                }
                else {
                    confirmExit = true;
                    Console.Write("\nConfirm shutdown? (y/N) ");
                }
            }

            Console.CancelKeyPress += OnCancelKeyPress;

            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                if (!exitToken.IsCancellationRequested)
                {
                    exitCts.Cancel();
                    Log.Warn("KILL");
                }
            };

            using var program = new Program(args);
            while (!exitToken.IsCancellationRequested)
            {
                await Task.Delay(100).ConfigureAwait(false);

                if (isDaemonMode)
                {
                    continue;
                }

                Console.Write("> ");
                var input = (await Console.In.ReadLineAsync(exitToken).ConfigureAwait(false))?.ToLower();

                if (confirmExit)
                {
                    if (input == "y")
                    {
                        exitCts.Cancel();
                    }
                    else
                    {
                        confirmExit = false;
                    }

                    continue;
                }

                switch (input)
                {
                    case "exit":
                        exitCts.Cancel();
                        break;
                    case "gc":
                        GC.Collect();
                        Console.WriteLine("Garbage Collected!");
                        break;
                    case "pid":
                        var process = Process.GetCurrentProcess();
                        var pid = process.Id.ToString();
                        Console.WriteLine($"PID: {pid}");
                        break;
                    case "?":
                    case "help":
                        var commandList = "exit, gc, pid";
                        Console.WriteLine($"Commands available: {commandList}");
                        break;
                    case "":
                    case null:
                        break;
                    default:
                        Console.WriteLine("Unknown command");
                        break;
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

            this.apiHost.Shutdown();
            this.adminPanel.Shutdown();

            (this.persistenceContextProvider as IDisposable)?.Dispose();
        }

        private void LoadGameClientDefinitions(IContext persistenceContext)
        {
            ClientVersionResolver.DefaultVersion = new ClientVersion(6, 3, ClientLanguage.English);
            foreach (var gameClientDefinition in persistenceContext.Get<GameClientDefinition>())
            {
                ClientVersionResolver.Register(
                    gameClientDefinition.Version,
                    new ClientVersion(gameClientDefinition.Season, gameClientDefinition.Episode, gameClientDefinition.Language));
            }
        }

        private void InitializeGameServers(IContext persistenceContext, IIpAddressResolver ipResolver)
        {
            this.LoadGameClientDefinitions(persistenceContext);
            var loginServer = new LoginServer();
            var chatServerDefinition = persistenceContext.Get<ChatServerDefinition>().First();
            var chatServer = new ChatServer(chatServerDefinition.ConvertToSettings(), ipResolver, chatServerDefinition.GetId());
            this.servers.Add(chatServer);
            var guildServer = new GuildServer(this.gameServers, this.persistenceContextProvider);
            var friendServer = new FriendServer(this.gameServers, chatServer, this.persistenceContextProvider);
            var connectServers = new Dictionary<GameClientDefinition, IGameServerStateObserver>();

            Log.Info("Start initializing game servers");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            foreach (var connectServerDefinition in persistenceContext.Get<ConnectServerDefinition>())
            {
                var clientVersion = new ClientVersion(connectServerDefinition.Client.Season, connectServerDefinition.Client.Episode, connectServerDefinition.Client.Language);
                var connectServer = ConnectServerFactory.CreateConnectServer(connectServerDefinition, clientVersion, connectServerDefinition.GetId());
                this.servers.Add(connectServer);
                if (!connectServers.TryGetValue(connectServerDefinition.Client, out var observer))
                {
                    connectServers[connectServerDefinition.Client] = connectServer;
                }
                else
                {
                    Log.WarnFormat($"Multiple connect servers for game client '{connectServerDefinition.Client.Description}' configured. Only one per client makes sense.");
                    if (!(observer is MulticastConnectionServerStateObserver))
                    {
                        var multicastObserver = new MulticastConnectionServerStateObserver();
                        multicastObserver.AddObserver(observer);
                        multicastObserver.AddObserver(connectServer);
                        connectServers[connectServerDefinition.Client] = multicastObserver;
                    }
                }
            }

            foreach (var gameServerDefinition in persistenceContext.Get<DataModel.Configuration.GameServerDefinition>())
            {
                using (ThreadContext.Stacks["gameserver"].Push(gameServerDefinition.ServerID.ToString()))
                {
                    var gameServer = new GameServer(gameServerDefinition, guildServer, loginServer, this.persistenceContextProvider, friendServer);
                    foreach (var endpoint in gameServerDefinition.Endpoints)
                    {
                        gameServer.AddListener(new DefaultTcpGameServerListener(endpoint, gameServer.ServerInfo, gameServer.Context, connectServers[endpoint.Client], ipResolver));
                    }

                    this.servers.Add(gameServer);
                    this.gameServers.Add(gameServer.Id, gameServer);
                    Log.InfoFormat($"Game Server {gameServer.Id} - [{gameServer.Description}] initialized");
                }
            }

            stopwatch.Stop();
            Log.Info($"All game servers initialized, elapsed time: {stopwatch.Elapsed}");
        }

        private ushort DetermineAdminPort(string[] args)
        {
            var parameter = args.FirstOrDefault(a => a.StartsWith("-adminport:", StringComparison.InvariantCultureIgnoreCase));
            if (parameter != null
                && int.TryParse(parameter.Substring(parameter.IndexOf(':') + 1), out int port)
                && port >= 1
                && port <= ushort.MaxValue)
            {
                return (ushort)port;
            }

            return 1234; // Default port
        }

        private IPersistenceContextProvider DeterminePersistenceContextProvider(string[] args)
        {
            IPersistenceContextProvider contextProvider;
            if (args.Contains("-demo"))
            {
                contextProvider = new InMemoryPersistenceContextProvider();
                var initialization = new DataInitialization(contextProvider);
                initialization.CreateInitialData();
            }
            else
            {
                contextProvider = this.PrepareRepositoryManager(args.Contains("-reinit"), args.Contains("-autoupdate"));
            }

            return contextProvider;
        }

        private IPersistenceContextProvider PrepareRepositoryManager(bool reinit, bool autoupdate)
        {
            PersistenceContextProvider.InitializeSqlLogging();
            var manager = new PersistenceContextProvider();
            if (reinit || !manager.DatabaseExists())
            {
                Log.Info("The database is getting (re-)initialized...");
                manager.ReCreateDatabase();
                var initialization = new DataInitialization(manager);
                initialization.CreateInitialData();
                Log.Info("...initialization finished.");
            }
            else if (!manager.IsDatabaseUpToDate())
            {
                if (autoupdate)
                {
                    Console.WriteLine("The database needs to be updated before the server can be started. Updating...");
                    manager.ApplyAllPendingUpdates();
                    Console.WriteLine("The database has been successfully updated.");
                }
                else
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
            }
            else
            {
                // everything is fine and ready
            }

            return manager;
        }
    }
}
