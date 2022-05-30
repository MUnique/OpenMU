// <copyright file="AdminPanel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

using System.Threading;
using Blazored.Modal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Services;
using MUnique.OpenMU.Web.Map.Map;
using SixLabors.ImageSharp;
using SixLabors.Memory;

/// <summary>
/// The admin panel host class which provides a web server over ASP.NET Core Kestrel.
/// </summary>
/// <remarks>
/// This class is only used when running as all-in-one deployment.
/// </remarks>
public sealed class AdminPanel : IHostedService, IDisposable
{
    private readonly IList<IManageableServer> _servers;
    private readonly IMigratableDatabaseContextProvider _persistenceContextProvider;
    private readonly IConfigurationChangePublisher _changePublisher;
    private readonly ILoggerFactory _loggerFactory;
    private readonly AdminPanelSettings _settings;
    private readonly PlugInManager _plugInManager;
    private readonly SetupService _setupService;
    private readonly ILogger<AdminPanel> _logger;
    private IHost? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPanel" /> class.
    /// </summary>
    /// <param name="servers">All manageable servers, including game servers, connect servers etc.</param>
    /// <param name="persistenceContextProvider">The persistence context provider.</param>
    /// <param name="changePublisher">The change listener.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="settings">The admin panel settings.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    /// <param name="setupService">The setup service.</param>
    public AdminPanel(IList<IManageableServer> servers, IMigratableDatabaseContextProvider persistenceContextProvider, IConfigurationChangePublisher changePublisher, ILoggerFactory loggerFactory, AdminPanelSettings settings, PlugInManager plugInManager, SetupService setupService)
    {
        this._servers = servers;
        this._persistenceContextProvider = persistenceContextProvider;
        this._changePublisher = changePublisher;
        this._loggerFactory = loggerFactory;
        this._settings = settings;
        this._plugInManager = plugInManager;
        this._setupService = setupService;
        this._logger = this._loggerFactory.CreateLogger<AdminPanel>();
    }

    /// <summary>
    /// Gets a value indicating whether the hosting of the admin panel is embedded into an all-in-one deployment.
    /// </summary>
    public static bool IsHostingEmbedded { get; private set; }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"Start initializing admin panel for port {this._settings.Port}.");
        Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();
        IsHostingEmbedded = true;

        // you might need to allow it first with netsh:
        // netsh http add urlacl http://+:1234/ user=[Username]
        this._host = Host.CreateDefaultBuilder()
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.AddSingleton(this._servers);
                serviceCollection.AddSingleton(this._setupService);
                serviceCollection.AddSingleton<IServerProvider, LocalServerProvider>();
                serviceCollection.AddSingleton(this._persistenceContextProvider);
                serviceCollection.AddSingleton<IPersistenceContextProvider>(this._persistenceContextProvider);
                serviceCollection.AddSingleton(this._changePublisher);
                serviceCollection.AddSingleton(this._loggerFactory);
                serviceCollection.AddSingleton(this._plugInManager);
                serviceCollection.AddScoped<IMapFactory, JavascriptMapFactory>();
                serviceCollection.AddBlazoredModal();
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.UseStaticWebAssets();

                // For testing purposes, we use http. Later we need to switch to https.
                // The self-signed certificate would otherwise cause a lot of warnings in the browser.
                webBuilder.UseUrls($"http://*:{this._settings.Port}");
            })
            .Build();
        await this._host!.StartAsync(cancellationToken);
        this._logger.LogInformation($"Admin panel initialized, port {this._settings.Port}.");
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation("Stopping admin panel");
        return this._host?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._host?.Dispose();
    }
}