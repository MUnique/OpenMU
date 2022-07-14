// <copyright file="LoginStateCleanup.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer.Host;

using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// A <see cref="IHostedService"/> which cleans up logged in accounts of
/// the game servers from the <see cref="PersistentLoginServer"/>, when a game server
/// is getting removed from the <see cref="GameServerRegistry"/>.
/// </summary>
public sealed class LoginStateCleanup : IHostedService
{
    private readonly GameServerRegistry _registry;
    private readonly PersistentLoginServer _loginServer;
    private readonly ILogger<LoginStateCleanup> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginStateCleanup"/> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="loginServer">The login server.</param>
    /// <param name="logger">The logger.</param>
    public LoginStateCleanup(GameServerRegistry registry, PersistentLoginServer loginServer, ILogger<LoginStateCleanup> logger)
    {
        this._registry = registry;
        this._loginServer = loginServer;
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._registry.NewGameServerAdded += this.OnNewGameServerAddedAsync;
        this._registry.GameServerRemoved += this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._registry.NewGameServerAdded -= this.OnNewGameServerAddedAsync;
        this._registry.GameServerRemoved -= this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    /// <summary>
    /// It's called when a new server which recently started, is added.
    /// We clean up the login states for this server as well.
    /// </summary>
    /// <param name="serverId">The id of the started server.</param>
    /// <remarks>
    /// We handle here the <see cref="GameServerRegistry.NewGameServerAdded"/> instead of the <see cref="GameServerRegistry.GameServerAdded"/>,
    /// because only then it makes sense to clean the login states of this server.
    /// Otherwise, it might be possible, that the login server itself just crashed and recognized a longer running game server. In that case, cleaning the states is not wanted.
    /// </remarks>
    private async ValueTask OnNewGameServerAddedAsync(ushort serverId)
    {
        try
        {
            await this._loginServer.RemoveServerAsync((byte)serverId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when adding server {0}", serverId);
        }
    }

    private async ValueTask OnGameServerRemovedAsync( ushort serverId)
    {
        try
        {
            await this._loginServer.RemoveServerAsync((byte)serverId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when removing server {0}", serverId);
        }
    }
}