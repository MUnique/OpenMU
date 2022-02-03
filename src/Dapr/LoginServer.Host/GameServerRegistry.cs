using System.Threading;
using Microsoft.Extensions.Logging;

namespace MUnique.OpenMU.LoginServer.Host;

public sealed class GameServerRegistry : IDisposable
{
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);
    private readonly TimeSpan _newServerUptimeLimit = TimeSpan.FromSeconds(60);
    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<GameServerRegistry> _logger;
    private readonly Dictionary<ushort, DateTime> _entries = new();
    private readonly SemaphoreSlim _semaphore = new(1);

    public GameServerRegistry(ILogger<GameServerRegistry> logger)
    {
        this._logger = logger;

        Task.Run(async () =>
        {
            try
            {
                await this.CleanupLoopAsync(this._disposeCts.Token);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in cleanup loop");
            }
        });
    }

    public event EventHandler<ushort>? GameServerAdded;

    public event EventHandler<ushort>? NewGameServerAdded;

    public event EventHandler<ushort>? GameServerRemoved;

    public void Dispose()
    {
        this._disposeCts.Cancel();
        this._disposeCts.Dispose();
        this._semaphore.Dispose();
    }
    
    public async Task UpdateRegistrationAsync(ushort gameServerId, TimeSpan uptime)
    {
        await this._semaphore.WaitAsync();
        try
        {
            var timestamp = DateTime.UtcNow;
            if (this._entries.TryAdd(gameServerId, timestamp))
            {
                if (uptime <= this._newServerUptimeLimit)
                {
                    this.NewGameServerAdded?.Invoke(this, gameServerId);
                }
                else
                {
                    this.GameServerAdded?.Invoke(this, gameServerId);
                }
            }
            else
            {
                this._entries[gameServerId] = timestamp;
            }
        }
        finally
        {
            this._semaphore.Release(1);
        }
    }

    private async Task CleanupLoopAsync(CancellationToken cancellationToken)
    {
        var tempRemoved = new List<ushort>();
        while (!this._disposeCts.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken);
            await this._semaphore.WaitAsync(cancellationToken);
            try
            {
                foreach (var serverId in this._entries.Keys)
                {
                    var lastUpdate = this._entries[serverId];
                    var diff = DateTime.UtcNow - lastUpdate;
                    if (diff > this._timeout)
                    {
                        this._logger.LogInformation("Difference of {0} higher than timeout for server {1}", diff, serverId);
                        this.GameServerRemoved?.Invoke(this, serverId);
                        tempRemoved.Add(serverId);
                    }
                }

                foreach (var serverId in tempRemoved)
                {
                    this._entries.Remove(serverId, out _);
                }
            }
            finally
            {
                this._semaphore.Release(1);
            }
        }
    }
}