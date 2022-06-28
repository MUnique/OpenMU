// <copyright file="MapApp.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Map;

using Microsoft.AspNetCore.Builder;
using MUnique.OpenMU.Web.Map.Map;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.Memory;

/// <summary>
/// The admin panel host class which provides a web server over ASP.NET Core Kestrel.
/// </summary>
[Obsolete]
public sealed class MapApp : IHostedService, IDisposable
{
    private const int StartPort = 4800;
    private readonly IObservableGameServer _gameServer;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<MapApp> _logger;
    private IHost? _host;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapApp" /> class.
    /// </summary>
    /// <param name="gameServer">The game server which contains the hosted maps.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public MapApp(IObservableGameServer gameServer, ILoggerFactory loggerFactory)
    {
        this._gameServer = gameServer;
        this._loggerFactory = loggerFactory;
        this._logger = this._loggerFactory.CreateLogger<MapApp>();
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var port = StartPort + this._gameServer.Id;

        this._logger.LogInformation($"Start initializing Map app for game server {this._gameServer.Id} on port {port}.");
        Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithMinimalPooling();

        var builder = WebApplication.CreateBuilder();
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddControllers();

        builder.Services
            .AddSingleton(this._gameServer)
                .AddSingleton(this._loggerFactory)
                .AddScoped<IMapFactory, JavascriptMapFactory>();

        var app = builder.Build();

        // Unfortunately, using builder.WebHost.UseUrls(...) will be overwritten by environment variables when calling builder.Build().
        // Because we use the MapApp as an addition to another WebApplication, we want our own port.
        // Maybe, when we got some time, we could integrate the page route within the normal DaprService.
        app.Configuration["urls"] = $"http://*:{port}";

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseStaticFiles();
        app.UseRouting();

        app.MapBlazorHub();
        app.MapControllers();
        app.MapRazorPages();
        app.MapFallbackToPage("/_Host");

        this._host = app;
        await this._host!.StartAsync(cancellationToken);
        this._logger.LogInformation($"Map app initialized for game server {this._gameServer.Id} on port {port}.");
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._logger.LogInformation($"Stopping Map app for game server {this._gameServer.Id}");
        return this._host?.StopAsync(cancellationToken) ?? Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        this._host?.Dispose();
    }
}