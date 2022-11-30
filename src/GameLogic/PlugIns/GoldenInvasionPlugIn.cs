// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Diagnostics;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

[PlugIn(nameof(GoldenInvasionPlugIn), "Handle Golden Invasion event")]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn
{
    private static readonly short LorenciaId = 0;
    private static readonly short NoriaId = 3;
    private static readonly short DeviasId = 2;

    private static readonly int DelayBeforeSpawn = 3 * 1000; // 3 secs
    private static readonly int Duration = 5 * 60 * 1000; // 5 minutes

    enum State
    {
        NotStarted,
        Initialized,
        Started,
    }

    private readonly Random _random = new();

    private DateTime _nextRunUtc = DateTime.UtcNow;

    private State _state = State.NotStarted;

    private int _map = LorenciaId;

    private short[] PossibleMaps => new[] { LorenciaId, NoriaId, DeviasId };

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        if (this._nextRunUtc > DateTime.UtcNow)
        {
            return;
        }

        switch (this._state)
        {
            case State.NotStarted:
                {
                    this._nextRunUtc = DateTime.UtcNow.AddMilliseconds(DelayBeforeSpawn);
                    this._map = this.PossibleMaps[this._random.Next(0, this.PossibleMaps.Length)];
                    this._state = State.Initialized;

                    var mapName = gameContext.Configuration.Maps.First(m => m.Number == this._map).Name;

                    Console.WriteLine($"[Golden invasion] starts at {mapName}");

                    try
                    {
                        await gameContext.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p, mapName)).ConfigureAwait(false);
                        await gameContext.ForEachPlayerAsync(p => this.TrySendFlyingDragonsAsync(p)).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.Message, ex.StackTrace);
                    }

                    break;
                }

            case State.Initialized:
                {
                    Console.WriteLine($"[Golden invasion] spawn mobs...");
                    this._nextRunUtc = DateTime.UtcNow.AddMilliseconds(Duration);
                    this._state = State.Started;
                    //TODO spawn
                    break;
                }
        }
    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        try
        {
            if (addedObject is Player player)
            {
                await this.TrySendFlyingDragonsAsync(player).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    private async Task TrySendStartMessageAsync(Player player, string mapName)
    {
        if (player.CurrentMap is { } map
           && player.PlayerState.CurrentState != PlayerState.Disconnected
           && player.PlayerState.CurrentState != PlayerState.Finished)
        {
            try
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync($"[{mapName}] Golden Invasion!", Interfaces.MessageType.GoldenCenter)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending start message.");
            }
        }
    }

    private async Task TrySendFlyingDragonsAsync(Player player)
    {
        if (player.CurrentMap is { } map
            && player.PlayerState.CurrentState != PlayerState.Disconnected
            && player.PlayerState.CurrentState != PlayerState.Finished
            && map.MapId == this._map)
        {
            try
            {
                await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(true, MapEventType.GoldenDragonInvasion)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending flying dragons update.");
            }
        }
    }
}