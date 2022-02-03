using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MUnique.OpenMU.LoginServer.Host;

public sealed class LoginStateCleanup : IHostedService
{
    private readonly GameServerRegistry _registry;
    private readonly PersistentLoginServer _loginServer;
    private readonly ILogger<LoginStateCleanup> _logger;

    public LoginStateCleanup(GameServerRegistry registry, PersistentLoginServer loginServer, ILogger<LoginStateCleanup> logger)
    {
        _registry = registry;
        _loginServer = loginServer;
        _logger = logger;
    }

    /// <summary>
    /// It's called when a new server which recently started, is added.
    /// We clean up the login states for this server as well.
    /// </summary>
    /// <param name="sender">The sender of the event.</param>
    /// <param name="serverId">The id of the started server.</param>
    /// <remarks>
    /// We handle here the <see cref="GameServerRegistry.NewGameServerAdded"/> instead of the <see cref="GameServerRegistry.GameServerAdded"/>,
    /// because only then it makes sense to clean the login states of this server.
    /// Otherwise, it might be possible, that the login server itself just crashed and recognized a longer running game server. In that case, cleaning the states is not wanted.
    /// </remarks>
    private async void OnNewGameServerAdded(object? sender, ushort serverId)
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

    private async void OnGameServerRemoved(object? sender, ushort serverId)
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

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _registry.NewGameServerAdded += this.OnNewGameServerAdded;
        _registry.GameServerRemoved += this.OnGameServerRemoved;
        
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _registry.NewGameServerAdded -= this.OnNewGameServerAdded;
        _registry.GameServerRemoved -= this.OnGameServerRemoved;
        
        return Task.CompletedTask;
    }
}