// <copyright file="WeatherUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the state of the weather of each hosted map in a random way and notifies players which enter a map about the current weather.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.WeatherUpdatePlugIn_Name), Description = nameof(PlugInResources.WeatherUpdatePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("3E702A15-653A-48EF-899C-4CDB2239A90C")]
public class WeatherUpdatePlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn
{
    /// <summary>
    /// The weather of the maps of each game server, which existed at its last run. The plugin instance is shared by
    /// the game servers of a process, and every server has its own maps. It's replaced as a whole on every run, so that
    /// a map which is gone in the meantime (e.g. the map of a finished mini game) isn't kept alive by it.
    /// </summary>
    private readonly ConditionalWeakTable<IGameContext, IReadOnlyDictionary<GameMap, (byte, byte)>> _weatherStates = new();

    /// <summary>
    /// The time of the next weather change, for each game server.
    /// </summary>
    private readonly PerServerSchedule _schedule = new();

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        // Taken for longer than a run lasts, so that the next calls of the timer don't start another one meanwhile.
        // The next run is scheduled from the end of this one.
        if (!this._schedule.TryStartRun(gameContext, TimeSpan.FromMinutes(1)))
        {
            return;
        }

        try
        {
            var weatherStates = new Dictionary<GameMap, (byte, byte)>();
            foreach (var map in await gameContext.GetMapsAsync().ConfigureAwait(false))
            {
                var weather = (byte)Rand.NextInt(0, 3);
                var variation = (byte)Rand.NextInt(0, 10);
                weatherStates[map] = (weather, variation);
            }

            this._weatherStates.AddOrUpdate(gameContext, weatherStates);

            await gameContext.ForEachPlayerAsync(this.TrySendPlayerUpdateAsync).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
        finally
        {
            this._schedule.ScheduleNextRun(gameContext, TimeSpan.FromSeconds(Rand.NextInt(10, 20)));
        }
    }

    /// <inheritdoc />
    public void ForceStart()
    {
        this._schedule.ForceStart();
    }

    /// <inheritdoc />
    public async ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is not Player player)
        {
            return;
        }

        try
        {
            await this.TrySendPlayerUpdateAsync(player).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending weather update.");
        }
    }

    private async Task TrySendPlayerUpdateAsync(Player player)
    {
        if (player.CurrentMap is { } map
            && !player.PlayerState.CurrentState.IsDisconnectedOrFinished()
            && this._weatherStates.TryGetValue(player.GameContext, out var weatherStates)
            && weatherStates.TryGetValue(map, out var weather))
        {
            try
            {
                await player.InvokeViewPlugInAsync<IWeatherStatusUpdatePlugIn>(p => p.ShowWeatherAsync(weather.Item1, weather.Item2)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending weather update.");
            }
        }
    }
}