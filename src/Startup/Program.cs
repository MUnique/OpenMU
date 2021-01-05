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
    using apache.log4net.Extensions.Logging;
    using log4net;
    using log4net.Config;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AdminPanel;
    using MUnique.OpenMU.AdminPanel.Services;
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
    using MUnique.OpenMU.PlugIns;
    using MUnique.OpenMU.PublicApi;
    using Nito.AsyncEx.Synchronous;

    /// <summary>
    /// The startup class for an all-in-one game server.
    /// </summary>
    internal sealed class Program : IDisposable
    {
        private static readonly string Log4NetConfigFilePath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + typeof(Program).GetTypeInfo().Namespace + ".exe.log4net.xml";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static bool confirmExit;
        private readonly IDictionary<int, IGameServer> gameServers = new Dictionary<int, IGameServer>();
        private readonly IList<IManageableServer> servers = new List<IManageableServer>();

        private IHost? serverHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        /// <param name="args">The command line args.</param>
        public async Task Initialize(string[] args)
        {
            Log.Info("Creating host...");
            this.serverHost = await this.CreateHost(args);

            if (args.Contains("-autostart") || !this.IsAdminPanelEnabled(args))
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
            var isDaemonMode = args.Contains("-daemon");

            void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
            {
                if (confirmExit)
                {
                    exitCts.Cancel();
                    Console.CancelKeyPress -= OnCancelKeyPress;
                    Console.WriteLine("\nBye! Press enter to finish");
                }
                else
                {
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

            using var program = new Program();
            await program.Initialize(args);
            while (!exitToken.IsCancellationRequested)
            {
                await Task.Delay(100).ConfigureAwait(false);

                if (isDaemonMode)
                {
                    continue;
                }

                await HandleConsoleInputAsync(exitCts, exitToken).ConfigureAwait(false);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.serverHost?.StopAsync().WaitAndUnwrapException();
            this.serverHost?.Dispose();
        }

        private static async Task HandleConsoleInputAsync(CancellationTokenSource exitCts, CancellationToken exitToken)
        {
            Console.Write("> ");
            var input = (await Console.In.ReadLineAsync(exitToken).ConfigureAwait(false))?.ToLower();

            switch (input)
            {
                case "y" when confirmExit:
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

            if (confirmExit && !string.IsNullOrWhiteSpace(input))
            {
                confirmExit = false;
            }
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

        private async Task<IHost> CreateHost(string[] args)
        {
            // Ensure GameLogic and GameServer Assemblies are loaded
            _ = GameLogic.Rand.NextInt(1, 2);
            _ = OpenMU.GameServer.ClientVersionResolver.DefaultVersion;

            var host = Host.CreateDefaultBuilder()
                .ConfigureLogging(configureLogging =>
                {
                    configureLogging.ClearProviders();
                    var settings = new Log4NetSettings { ConfigFile = Log4NetConfigFilePath, Watch = true };
                    configureLogging.AddLog4Net(settings);
                })
                .ConfigureServices(c =>
                {
                    c.AddSingleton(this.servers)
                        .AddSingleton(s => s.GetService<IPersistenceContextProvider>()?.CreateNewConfigurationContext().Get<ChatServerDefinition>().First() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."))
                        .AddSingleton(s => s.GetService<ChatServerDefinition>()?.ConvertToSettings() ?? throw new Exception($"{nameof(ChatServerSettings)} not registered."))
                        .AddIpResolver(args)
                        .AddSingleton(this.gameServers)
                        .AddSingleton(this.gameServers.Values)
                        .AddSingleton(s => this.DeterminePersistenceContextProvider(args, s.GetService<ILoggerFactory>() ?? throw new Exception($"{nameof(ILoggerFactory)} not registered.")))
                        .AddSingleton<ILoginServer, LoginServer>()
                        .AddSingleton<IGuildServer, GuildServer>()
                        .AddSingleton<IFriendServer, FriendServer>()
                        .AddSingleton<IChatServer, ChatServer>()
                        .AddSingleton<ConnectServerFactory>()
                        .AddSingleton<ConnectServerContainer>()
                        .AddSingleton<IEnumerable<IConnectServer>>(provider => provider.GetService<ConnectServerContainer>() ?? throw new Exception($"{nameof(ConnectServerContainer)} not registered."))
                        .AddSingleton<GameServerContainer>()
                        .AddSingleton<PlugInManager>()
                        .AddSingleton<ICollection<PlugInConfiguration>>(s => s.GetService<IPersistenceContextProvider>()?.CreateNewTypedContext<PlugInConfiguration>().Get<PlugInConfiguration>().ToList() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."))
                        .AddHostedService(provider => provider.GetService<IChatServer>())
                        .AddHostedService(provider => provider.GetService<ConnectServerContainer>())
                        .AddHostedService(provider => provider.GetService<GameServerContainer>());
                    if (this.IsAdminPanelEnabled(args))
                    {
                        c.AddSingleton<IServerConfigurationChangeListener, ServerConfigurationChangeListener>()
                            .AddSingleton<IPlugInConfigurationChangeListener, PlugInConfigurationChangeListener>()
                            .AddHostedService<AdminPanel>()
                            .AddSingleton(new AdminPanelSettings(this.DetermineAdminPort(args)));
                    }

                    if (this.IsApiEnabled(args))
                    {
                        c.AddHostedService<ApiHost>();
                    }
                })
                .Build();
            Log.Info("Host created");
            if (host.Services.GetService<ILoggerFactory>() is { } loggerFactory)
            {
                NpgsqlLoggingProvider.Initialize(loggerFactory);
            }

            this.servers.Add(host.Services.GetService<IChatServer>() ?? throw new Exception($"{nameof(IChatServer)} not registered."));
            this.LoadGameClientDefinitions(host.Services.GetService<IPersistenceContextProvider>()?.CreateNewConfigurationContext() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."));
            Log.Info("Starting host...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await host.StartAsync();
            stopwatch.Stop();
            Log.Info($"Host started, elapsed time: {stopwatch.Elapsed}");
            return host;
        }

        private ushort DetermineAdminPort(string[] args) => this.DetermineUshort("adminport", args, 1234);

        private ushort DetermineUshort(string parameterName, string[] args, ushort defaultValue)
        {
            var parameter = args.FirstOrDefault(a => a.StartsWith($"-{parameterName}:", StringComparison.InvariantCultureIgnoreCase));
            if (parameter != null
                && int.TryParse(parameter.Substring(parameter.IndexOf(':') + 1), out int value)
                && value >= 0
                && value <= ushort.MaxValue)
            {
                return (ushort)value;
            }

            return defaultValue;
        }

        private bool IsAdminPanelEnabled(string[] args) => this.IsFeatureEnabled("adminpanel", args);

        private bool IsApiEnabled(string[] args) => this.IsFeatureEnabled("api", args);

        private bool IsFeatureEnabled(string featureName, string[] args)
        {
            var parameter = args.FirstOrDefault(a => a.StartsWith($"-{featureName}:", StringComparison.InvariantCultureIgnoreCase));
            if (parameter is null)
            {
                return true;
            }

            return parameter.Substring(parameter.IndexOf(':') + 1).StartsWith("enabled", StringComparison.InvariantCultureIgnoreCase);
        }

        private IPersistenceContextProvider DeterminePersistenceContextProvider(string[] args, ILoggerFactory loggerFactory)
        {
            IPersistenceContextProvider contextProvider;
            if (args.Contains("-demo"))
            {
                contextProvider = new InMemoryPersistenceContextProvider();
                var initialization = new DataInitialization(contextProvider, loggerFactory);
                initialization.CreateInitialData();
            }
            else
            {
                contextProvider = this.PrepareRepositoryManager(args.Contains("-reinit"), args.Contains("-autoupdate"), loggerFactory);
            }

            return contextProvider;
        }

        private IPersistenceContextProvider PrepareRepositoryManager(bool reinit, bool autoupdate, ILoggerFactory loggerFactory)
        {
            var manager = new PersistenceContextProvider(loggerFactory);
            if (reinit || !manager.DatabaseExists())
            {
                Log.Info("The database is getting (re-)initialized...");
                manager.ReCreateDatabase();
                var initialization = new DataInitialization(manager, loggerFactory);
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
                        return null!;
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
