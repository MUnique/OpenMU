// <copyright file="MiniGameSpawnWaveRunner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Runs the spawn waves of a mini game.
/// All configured waves are started as tasks, because they may overlap.
/// </summary>
internal sealed class MiniGameSpawnWaveRunner
{
    private readonly MiniGameDefinition _definition;
    private readonly GameMap _map;
    private readonly IMapInitializer _mapInitializer;
    private readonly IEventStateProvider _eventStateProvider;
    private readonly ILogger _logger;
    private readonly object _owner;
    private readonly Func<LocalizedString, ValueTask> _announceMessageAsync;
    private readonly Func<TimeSpan?> _elapsedProvider;
    private readonly ConcurrentDictionary<byte, MiniGameSpawnWave> _currentSpawnWaves = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniGameSpawnWaveRunner"/> class.
    /// </summary>
    /// <param name="definition">The definition of the mini game.</param>
    /// <param name="map">The map on which the game takes place.</param>
    /// <param name="mapInitializer">The map initializer, which is used to initialize the wave spawns.</param>
    /// <param name="eventStateProvider">The event state provider, handed to the map initializer.</param>
    /// <param name="logger">The logger for this instance.</param>
    /// <param name="owner">The owner which is named in the log entries.</param>
    /// <param name="announceMessageAsync">Shows a golden center-screen message to all players of the game.</param>
    /// <param name="elapsedProvider">A function which returns the elapsed time since the game started.</param>
    public MiniGameSpawnWaveRunner(
        MiniGameDefinition definition,
        GameMap map,
        IMapInitializer mapInitializer,
        IEventStateProvider eventStateProvider,
        ILogger logger,
        object owner,
        Func<LocalizedString, ValueTask> announceMessageAsync,
        Func<TimeSpan?> elapsedProvider)
    {
        this._definition = definition;
        this._map = map;
        this._mapInitializer = mapInitializer;
        this._eventStateProvider = eventStateProvider;
        this._logger = logger;
        this._owner = owner;
        this._announceMessageAsync = announceMessageAsync;
        this._elapsedProvider = elapsedProvider;
    }

    /// <summary>
    /// Determines if the spawn wave with the given number is currently active.
    /// </summary>
    /// <param name="waveNumber">The number of the wave.</param>
    /// <returns><c>true</c> if the wave is active; otherwise, <c>false</c>.</returns>
    public bool IsActive(byte waveNumber)
    {
        return this._currentSpawnWaves.ContainsKey(waveNumber);
    }

    /// <summary>
    /// Clears all currently active spawn waves.
    /// </summary>
    public void Clear()
    {
        this._currentSpawnWaves.Clear();
    }

    /// <summary>
    /// Runs all spawn waves until the <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            // We already need to start all tasks, because they may overlap.
            // So it's not okay to run them one after another.
            var waveTasks = this._definition.SpawnWaves
                .OrderBy(wave => wave.WaveNumber)
                .Select(wave => this.RunSpawnWaveAsync(wave, cancellationToken))
                .ToList();
            await Task.WhenAll(waveTasks).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // game ended.
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error during spawn waves: {0}", ex.Message);
        }
    }

    private async Task RunSpawnWaveAsync(MiniGameSpawnWave spawnWave, CancellationToken cancellationToken)
    {
        try
        {
            while (spawnWave.StartTime > this._elapsedProvider())
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Wait at least a second (or half of the remaining time) to the next check
                var timeUntilNextCheck = (spawnWave.StartTime - this._elapsedProvider()!.Value) / 2;
                var requiredDelay = timeUntilNextCheck > TimeSpan.FromSeconds(1)
                    ? timeUntilNextCheck
                    : TimeSpan.FromSeconds(1);
                await Task.Delay(requiredDelay, cancellationToken).ConfigureAwait(false);
            }

            this._logger.LogDebug("{context}: Starting next wave: {wave}", this._owner, spawnWave.Description);
            if (spawnWave.Message is { } message)
            {
                await this._announceMessageAsync(message).ConfigureAwait(false);
            }

            if (!this._currentSpawnWaves.TryAdd(spawnWave.WaveNumber, spawnWave))
            {
                this._logger.LogWarning("{context}: Duplicate spawn wave number in event: {wave}. Check your configuration, every spawn wave needs a distinct number.", this._owner, spawnWave.Description);
            }

            await this._mapInitializer.InitializeNpcsOnWaveStartAsync(this._map, this._eventStateProvider, spawnWave.WaveNumber).ConfigureAwait(false);
            await Task.Delay(spawnWave.EndTime - spawnWave.StartTime, cancellationToken).ConfigureAwait(false);
            this._logger.LogDebug("{context}: Wave ended: {wave}", this._owner, spawnWave.Description);
        }
        catch (OperationCanceledException)
        {
            // do nothing, as it's expected when game ends ...
            throw;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error during spawn wave {0}: {1}", spawnWave.WaveNumber, ex.Message);
        }
        finally
        {
            this._currentSpawnWaves.Remove(spawnWave.WaveNumber, out _);
        }
    }
}
