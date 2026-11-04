// <copyright file="OnlinePlayerHistory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Keeps the history of the number of online players of the last <see cref="Window"/>,
/// by sampling the game servers of the <see cref="IServerProvider"/> every <see cref="SampleInterval"/>.
/// </summary>
/// <remarks>
/// The player count is also exported as a metric of the game context (see GameContext.PlayerCount),
/// but metrics are only observable in the process which records them. The admin panel can run in
/// its own process (distributed deployment), so we sample the <see cref="IServerProvider"/>,
/// which provides the player counts in both deployments.
/// The history is kept in memory, so it starts over when the admin panel is restarted.
/// </remarks>
public sealed class OnlinePlayerHistory : BackgroundService
{
    /// <summary>
    /// The interval in which the number of online players is sampled.
    /// </summary>
    public static readonly TimeSpan SampleInterval = TimeSpan.FromMinutes(5);

    /// <summary>
    /// The time window which is kept in the history.
    /// </summary>
    public static readonly TimeSpan Window = TimeSpan.FromHours(24);

    private readonly IServerProvider _serverProvider;

    private readonly TimeProvider _timeProvider;

    private readonly ILogger<OnlinePlayerHistory> _logger;

    private readonly Queue<OnlinePlayerSample> _samples = new();

    private readonly object _syncRoot = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlinePlayerHistory"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="logger">The logger.</param>
    public OnlinePlayerHistory(IServerProvider serverProvider, ILogger<OnlinePlayerHistory> logger)
        : this(serverProvider, TimeProvider.System, logger)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlinePlayerHistory"/> class.
    /// </summary>
    /// <param name="serverProvider">The server provider.</param>
    /// <param name="timeProvider">The time provider.</param>
    /// <param name="logger">The logger.</param>
    internal OnlinePlayerHistory(IServerProvider serverProvider, TimeProvider timeProvider, ILogger<OnlinePlayerHistory> logger)
    {
        this._serverProvider = serverProvider;
        this._timeProvider = timeProvider;
        this._logger = logger;
    }

    /// <summary>
    /// Occurs when a sample was added.
    /// </summary>
    public event EventHandler? SampleAdded;

    /// <summary>
    /// Gets the samples of the last <see cref="Window"/>, ordered by their timestamp.
    /// </summary>
    /// <returns>The samples of the last <see cref="Window"/>.</returns>
    public IReadOnlyList<OnlinePlayerSample> GetSamples()
    {
        lock (this._syncRoot)
        {
            return this._samples.ToList();
        }
    }

    /// <summary>
    /// Takes a sample of the current number of online players.
    /// </summary>
    internal void TakeSample()
    {
        var now = this._timeProvider.GetUtcNow().UtcDateTime;
        var sample = new OnlinePlayerSample(now, ServerStatistics.GetOnlinePlayerCount(this._serverProvider.Servers));
        lock (this._syncRoot)
        {
            this._samples.Enqueue(sample);
            while (this._samples.Peek().Timestamp < now - Window)
            {
                this._samples.Dequeue();
            }
        }

        this.SampleAdded?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(SampleInterval, this._timeProvider);
        do
        {
            try
            {
                this.TakeSample();
            }
            catch (Exception ex)
            {
                // An exception must not end the sampling, or even stop the host.
                this._logger.LogError(ex, "Couldn't take a sample of the online players.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false));
    }
}
