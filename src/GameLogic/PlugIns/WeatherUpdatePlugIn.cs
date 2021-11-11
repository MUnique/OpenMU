// <copyright file="WeatherUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Diagnostics;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Updates the state of the weather of each hosted map in a random way and notifies players which enter a map about the current weather.
/// </summary>
[PlugIn(nameof(WeatherUpdatePlugIn), "Updates the state of the weather of each hosted map in a random way.")]
[Guid("3E702A15-653A-48EF-899C-4CDB2239A90C")]
public class WeatherUpdatePlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn
{
    private readonly Random _random = new ();

    private readonly IDictionary<GameMap, (byte, byte)> _weatherStates = new Dictionary<GameMap, (byte, byte)>();

    private DateTime _nextRunUtc = DateTime.UtcNow;

    private bool _isRunning;

    /// <inheritdoc />
    public void ExecuteTask(GameContext gameContext)
    {
        if (this._nextRunUtc > DateTime.UtcNow || this._isRunning)
        {
            return;
        }

        this._isRunning = true;
        try
        {
            foreach (var map in gameContext.Maps)
            {
                var weather = (byte)this._random.Next(0, 3);
                var variation = (byte)this._random.Next(0, 10);
                this._weatherStates[map] = (weather, variation);
            }

            gameContext.ForEachPlayer(this.TrySendPlayerUpdate);
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
        finally
        {
            this._isRunning = false;
            this._nextRunUtc = DateTime.UtcNow.AddSeconds(this._random.Next(10, 20));
        }
    }

    /// <inheritdoc />
    public void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        if (addedObject is Player player)
        {
            this.TrySendPlayerUpdate(player);
        }
    }

    private void TrySendPlayerUpdate(Player player)
    {
        if (player.CurrentMap is { } map
            && player.PlayerState.CurrentState != PlayerState.Disconnected
            && player.PlayerState.CurrentState != PlayerState.Finished
            && this._weatherStates.TryGetValue(map, out var weather))
        {
            try
            {
                player.ViewPlugIns.GetPlugIn<IWeatherStatusUpdatePlugIn>()?.ShowWeather(weather.Item1, weather.Item2);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending weather update.");
            }
        }
    }
}