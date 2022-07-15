// <copyright file="Program.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Startup;

using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx.Synchronous;
using Serilog;
using Serilog.Debugging;
using SixLabors.ImageSharp;
using SixLabors.Memory;
using MUnique.OpenMU.Web.AdminPanel;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Map.Map;
using MUnique.OpenMU.ChatServer;
using MUnique.OpenMU.ConnectServer;
using MUnique.OpenMU.FriendServer;
using MUnique.OpenMU.GuildServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.LoginServer;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.EntityFramework;
using MUnique.OpenMU.Persistence.Initialization;
using MUnique.OpenMU.Persistence.Initialization.Version075;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The startup class for an all-in-one game server.
/// </summary>
internal sealed class Program : IDisposable
{
    private static bool _confirmExit;
    private readonly IDictionary<int, IGameServer> _gameServers = new Dictionary<int, IGameServer>();
    private readonly IList<IManageableServer> _servers = new List<IManageableServer>();
    private readonly Serilog.ILogger _logger;

    private IHost? _serverHost;

    /// <summary>
    /// Initializes a new instance of the <see cref="Program"/> class.
    /// </summary>
    public Program()
    {
        SelfLog.Enable(Console.Error);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true, true)
            .Build();

        this._logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    /// <summary>
    /// The main method.
    /// </summary>
    /// <param name="args">The command line args.</param>
    public static async Task Main(string[] args)
    {
        Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();
        using var exitCts = new CancellationTokenSource();
        var exitToken = exitCts.Token;
        var isDaemonMode = args.Contains("-daemon");

        void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs e)
        {
            if (_confirmExit)
            {
                exitCts.Cancel();
                Console.CancelKeyPress -= OnCancelKeyPress;
                Console.WriteLine("\nBye! Press enter to finish");
            }
            else
            {
                _confirmExit = true;
                Console.Write("\nConfirm shutdown? (y/N) ");
            }
        }

        Console.CancelKeyPress += OnCancelKeyPress;

        AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
        {
            if (!exitToken.IsCancellationRequested)
            {
                exitCts.Cancel();
                Debug.WriteLine("KILL");
            }
        };

        using var program = new Program();
        await program.InitializeAsync(args).ConfigureAwait(false);
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

    /// <summary>
    /// Initializes a new instance of the <see cref="Program"/> class.
    /// </summary>
    /// <param name="args">The command line args.</param>
    public async Task InitializeAsync(string[] args)
    {
        this._logger.Information("Creating host...");
        this._serverHost = await this.CreateHostAsync(args).ConfigureAwait(false);

        if (args.Contains("-autostart") || !this.IsAdminPanelEnabled(args))
        {
            foreach (var chatServer in this._servers.OfType<ChatServer>())
            {
                await chatServer.StartAsync().ConfigureAwait(false);
            }

            foreach (var gameServer in this._gameServers.Values)
            {
                await gameServer.StartAsync().ConfigureAwait(false);
            }

            foreach (var connectServer in this._servers.OfType<IConnectServer>())
            {
                await connectServer.StartAsync().ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this._serverHost?.StopAsync().WaitAndUnwrapException();
        this._serverHost?.Dispose();
    }

    private static async Task HandleConsoleInputAsync(CancellationTokenSource exitCts, CancellationToken exitToken)
    {
        Console.Write("> ");
        var input = (await Console.In.ReadLineAsync(exitToken).ConfigureAwait(false))?.ToLower();

        switch (input)
        {
            case "y" when _confirmExit:
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

        if (_confirmExit && !string.IsNullOrWhiteSpace(input))
        {
            _confirmExit = false;
        }
    }

    private async Task<IHost> CreateHostAsync(string[] args)
    {
        // Ensure GameLogic and GameServer Assemblies are loaded
        _ = GameLogic.Rand.NextInt(1, 2);
        _ = DataInitialization.Id;
        _ = OpenMU.GameServer.ClientVersionResolver.DefaultVersion;

        var addAdminPanel = this.IsAdminPanelEnabled(args);
        await new ConfigFileDatabaseConnectionStringProvider().InitializeAsync(default).ConfigureAwait(false);

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog(this._logger);
        if (addAdminPanel)
        {
            builder.AddAdminPanel(includeMapApp: true);
        }

        builder.Host
            .ConfigureServices(c =>
            {
                c.AddSingleton(this._servers)
                    .AddSingleton<IConfigurationChangePublisher, ConfigurationChangeHandler>()
                    .AddIpResolver(args)
                    .AddSingleton(this._gameServers)
                    .AddSingleton(this._gameServers.Values)
                    .AddSingleton(s =>
                        this.DeterminePersistenceContextProviderAsync(
                            args,
                            s.GetService<ILoggerFactory>() ?? throw new Exception($"{nameof(ILoggerFactory)} not registered."))
                            .WaitAndUnwrapException())
                    .AddSingleton<IPersistenceContextProvider>(s => s.GetService<IMigratableDatabaseContextProvider>()!)
                    .AddSingleton<ILoginServer, LoginServer>()
                    .AddSingleton<IGuildServer, GuildServer>()
                    .AddSingleton<IFriendServer, FriendServer>()
                    .AddSingleton<ChatServer>()
                    .AddSingleton<IChatServer>(s => s.GetService<ChatServer>()!)
                    .AddSingleton<ConnectServerFactory>()
                    .AddSingleton<ConnectServerContainer>()
                    .AddScoped<IMapFactory, JavascriptMapFactory>()
                    .AddSingleton<SetupService>()
                    .AddSingleton<IEnumerable<IConnectServer>>(provider => provider.GetService<ConnectServerContainer>() ?? throw new Exception($"{nameof(ConnectServerContainer)} not registered."))
                    .AddSingleton<IGuildChangePublisher, GuildChangeToGameServerPublisher>()
                    .AddSingleton<IFriendNotifier, FriendNotifierToGameServer>()
                    .AddSingleton<PlugInManager>()
                    .AddSingleton<IServerProvider, LocalServerProvider>()
                    .AddSingleton<ICollection<PlugInConfiguration>>(s => s.GetService<IPersistenceContextProvider>()?.CreateNewTypedContext<PlugInConfiguration>().GetAsync<PlugInConfiguration>().AsTask().WaitAndUnwrapException().ToList() ?? throw new Exception($"{nameof(IPersistenceContextProvider)} not registered."))
                    .AddHostedService<ChatServerContainer>()
                    .AddHostedService<GameServerContainer>()
                    .AddHostedService(provider => provider.GetService<ConnectServerContainer>());
            });
        var host = builder.Build();

        this._logger.Information("Host created");
        if (host.Services.GetService<ILoggerFactory>() is { } loggerFactory)
        {
            NpgsqlLoggingProvider.Initialize(loggerFactory);
        }

        if (addAdminPanel)
        {
            host.ConfigureAdminPanel();
        }

        this._logger.Information("Starting host...");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        await host.StartAsync().ConfigureAwait(false);
        stopwatch.Stop();
        this._logger.Information($"Host started, elapsed time: {stopwatch.Elapsed}");
        return host;
    }

    private ushort DetermineUshort(string parameterName, string[] args, ushort defaultValue)
    {
        var parameter = args.FirstOrDefault(a => a.StartsWith($"-{parameterName}:", StringComparison.InvariantCultureIgnoreCase));
        if (parameter != null
            && int.TryParse(parameter.Substring(parameter.IndexOf(':') + 1), out int value)
            && value is >= 0 and <= ushort.MaxValue)
        {
            return (ushort)value;
        }

        return defaultValue;
    }

    private bool IsAdminPanelEnabled(string[] args) => this.IsFeatureEnabled("adminpanel", args);

    private bool IsFeatureEnabled(string featureName, string[] args)
    {
        var parameter = args.FirstOrDefault(a => a.StartsWith($"-{featureName}:", StringComparison.InvariantCultureIgnoreCase));
        if (parameter is null)
        {
            return true;
        }

        return parameter.Substring(parameter.IndexOf(':') + 1).StartsWith("enabled", StringComparison.InvariantCultureIgnoreCase);
    }

    private string GetVersionParameter(string[] args)
    {
        var parameter = args.FirstOrDefault(a => a.StartsWith("-version:", StringComparison.InvariantCultureIgnoreCase));
        if (parameter is null)
        {
            return "season6"; // default
        }

        return parameter.Substring(parameter.IndexOf(':') + 1);
    }

    private async Task<IMigratableDatabaseContextProvider> DeterminePersistenceContextProviderAsync(string[] args, ILoggerFactory loggerFactory)
    {
        var version = this.GetVersionParameter(args);

        IMigratableDatabaseContextProvider contextProvider;
        if (args.Contains("-demo"))
        {
            contextProvider = new InMemoryPersistenceContextProvider();
            await this.InitializeDataAsync(version, loggerFactory, contextProvider).ConfigureAwait(false);
        }
        else
        {
            contextProvider = await this.PrepareRepositoryManagerAsync(args.Contains("-reinit"), version, args.Contains("-autoupdate"), loggerFactory).ConfigureAwait(false);
        }

        return contextProvider;
    }

    private async Task<IMigratableDatabaseContextProvider> PrepareRepositoryManagerAsync(bool reinit, string version, bool autoupdate, ILoggerFactory loggerFactory)
    {
        var contextProvider = new PersistenceContextProvider(loggerFactory, null);
        if (reinit || !await contextProvider.DatabaseExistsAsync().ConfigureAwait(false))
        {
            this._logger.Information("The database is getting (re-)initialized...");
            await contextProvider.ReCreateDatabaseAsync().ConfigureAwait(false);
            await this.InitializeDataAsync(version, loggerFactory, contextProvider).ConfigureAwait(false);
            this._logger.Information("...initialization finished.");
        }
        else if (!await contextProvider.IsDatabaseUpToDateAsync().ConfigureAwait(false))
        {
            if (autoupdate)
            {
                Console.WriteLine("The database needs to be updated before the server can be started. Updating...");
                await contextProvider.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);
                Console.WriteLine("The database has been successfully updated.");
            }
            else
            {
                Console.WriteLine("The database needs to be updated before the server can be started. Apply update? (y/n)");
                var key = Console.ReadLine()?.ToLowerInvariant();
                if (key == "y")
                {
                    await contextProvider.ApplyAllPendingUpdatesAsync().ConfigureAwait(false);
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

        return contextProvider;
    }

    private async Task InitializeDataAsync(string version, ILoggerFactory loggerFactory, IPersistenceContextProvider contextProvider)
    {
        var serviceContainer = new ServiceContainer();
        serviceContainer.AddService(typeof(ILoggerFactory), loggerFactory);
        serviceContainer.AddService(typeof(IPersistenceContextProvider), contextProvider);

        var plugInManager = new PlugInManager(null, loggerFactory, serviceContainer);
        plugInManager.DiscoverAndRegisterPlugInsOf<IDataInitializationPlugIn>();
        var initialization = plugInManager.GetStrategy<IDataInitializationPlugIn>(version) ?? throw new Exception("Data initialization plugin not found");
        await initialization.CreateInitialDataAsync(3, true).ConfigureAwait(false);
    }
}