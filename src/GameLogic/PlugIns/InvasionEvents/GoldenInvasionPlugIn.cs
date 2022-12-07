// <copyright file="GoldenInvasionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Golden Invasion feature.
/// </summary>
[PlugIn(nameof(GoldenInvasionPlugIn), "Handle Golden Invasion event")]
[Guid("06D18A9E-2919-4C17-9DBC-6E4F7756495C")]
public class GoldenInvasionPlugIn : BaseInvasionPlugIn<PeriodicInvasionConfiguration>
{
    private const ushort AtlansId = 7;
    private const ushort TarkanId = 8;

    private const ushort GoldenBudgeDragonId = 43;
    private const ushort GoldenGoblinId = 78;
    private const ushort GoldenSoldierId = 54;
    private const ushort GoldenTitanId = 53;
    private const ushort GoldenDragonId = 79;
    private const ushort GoldenVeparId = 81;
    private const ushort GoldenLizardKingId = 80;
    private const ushort GoldenWheelId = 83;
    private const ushort GoldenTantallosId = 82;

    /// <inheritdoc />
    protected override (ushort MapId, ushort MonsterId, ushort Count)[] Mobs { get; } =
    {
        (LorenciaId, GoldenBudgeDragonId, 20),
        (NoriaId, GoldenGoblinId, 20),
        (DeviasId, GoldenSoldierId, 20),
        (DeviasId, GoldenTitanId, 10),
        (AtlansId, GoldenVeparId, 20),
        (AtlansId, GoldenLizardKingId, 10),
        (TarkanId, GoldenWheelId, 20),
        (TarkanId, GoldenTantallosId, 10),
    };

    /// <inheritdoc />
    protected override (ushort MonsterId, ushort Count)[] MobsOnSelectedMap { get; } =
    {
         (GoldenDragonId, 10),
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
        var state = GetStateByGameContext(player.GameContext);

        if (!IsPlayerOnMap(player, true))
        {
            return;
        }

        try
        {
            await player.InvokeViewPlugInAsync<IMapEventStateUpdatePlugIn>(p => p.UpdateStateAsync(enabled, MapEventType.GoldenDragonInvasion)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending flying dragons update.");
        }
    }
}
