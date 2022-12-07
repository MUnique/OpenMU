// <copyright file="RedDragonInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Red Dragon Invasion feature.
/// </summary>
[PlugIn(nameof(RedDragonInvasionPlugIn), "Handle red dragon invasion event")]
[Guid("548A76CC-242C-441C-BC9D-6C22745A2D72")]
public class RedDragonInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>
{
    private const ushort RedDragonId = 44;

    /// <inheritdoc />
    protected override (ushort MonsterId, ushort Count)[] MobsOnSelectedMap { get; } =
    {
         (RedDragonId, 5),
    };

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public override async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        try
        {
            if (addedObject is Player player)
            {
                var state = GetStateByGameContext(player.GameContext);

                var flyingEnabled = state.State != InvasionEventState.NotStarted;

                await TrySendFlyingDragonsAsync(player, flyingEnabled).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnPreparedAsync(GameServerState state)
    {
        await base.OnPreparedAsync(state).ConfigureAwait(false);

        await state.Context.ForEachPlayerAsync(p => TrySendFlyingDragonsAsync(p, true)).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask OnFinishedAsync(GameServerState state)
    {
        await base.OnFinishedAsync(state).ConfigureAwait(false);

        await state.Context.ForEachPlayerAsync(p => TrySendFlyingDragonsAsync(p, false)).ConfigureAwait(false);
    }

    private static async Task TrySendFlyingDragonsAsync(Player player, bool enabled)
    {
        if (!IsPlayerOnMap(player, true))
        {
            return;
        }

        try
        {
            await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, MapEventType.RedDragonInvasion)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending flying dragons update.");
        }
    }
}
