// <copyright file="CastleSiegeMachineUseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Fires a Castle Siege warfare machine at a configured target zone.
/// </summary>
public sealed class CastleSiegeMachineUseAction
{
    private static readonly TimeSpan ImpactDelay = TimeSpan.FromMilliseconds(1500);
    private readonly IRandomizer _randomizer;
    private readonly Func<TimeSpan, ValueTask> _delay;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineUseAction"/> class.
    /// </summary>
    public CastleSiegeMachineUseAction()
        : this(Rand.GetRandomizer(), static delay => new ValueTask(Task.Delay(delay)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineUseAction"/> class.
    /// </summary>
    /// <param name="randomizer">The coordinate randomizer.</param>
    /// <param name="delay">The impact delay implementation.</param>
    internal CastleSiegeMachineUseAction(IRandomizer randomizer, Func<TimeSpan, ValueTask> delay)
    {
        this._randomizer = randomizer;
        this._delay = delay;
    }

    /// <summary>
    /// Tries to fire a warfare machine at the requested target zone.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="machineId">The machine object identifier.</param>
    /// <param name="targetZoneIndex">The one-based target-zone index.</param>
    /// <returns><see langword="true"/> when the machine was fired; otherwise, <see langword="false"/>.</returns>
    public async ValueTask<bool> UseAsync(
        Player player,
        CastleSiegeContext? context,
        ushort machineId,
        byte targetZoneIndex)
    {
        if (context is not { Configuration.Enabled: true })
        {
            return false;
        }

        CastleSiegeMachine? machine = null;
        GameMap? map = null;
        CastleSiegeMachineType machineType = default;
        Point target = default;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.Start
                || !player.IsAlive
                || player.CurrentMap is not { } playerMap
                || playerMap.GetObject(machineId) is not CastleSiegeMachine foundMachine
                || !ReferenceEquals(foundMachine.Operator, player)
                || !foundMachine.CanBeUsedBy(context.GetPlayerJoinSide(player))
                || foundMachine.IsActive
                || targetZoneIndex == 0)
            {
                return false;
            }

            var zones = foundMachine.MachineType == CastleSiegeMachineType.Attack
                ? context.Configuration.AttackMachineZones
                : context.Configuration.DefenseMachineZones;
            if (targetZoneIndex > zones.Count)
            {
                return false;
            }

            var targetZone = zones.ElementAt(targetZoneIndex - 1);
            if (targetZone.X1 > targetZone.X2 || targetZone.Y1 > targetZone.Y2)
            {
                return false;
            }

            machine = foundMachine;
            map = playerMap;
            machineType = foundMachine.MachineType;
            target = new Point(
                checked((byte)this._randomizer.NextInt(targetZone.X1, targetZone.X2 + 1)),
                checked((byte)this._randomizer.NextInt(targetZone.Y1, targetZone.Y2 + 1)));
            machine.IsActive = true;
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        try
        {
            await player.ForEachWorldObserverAsync<ICastleSiegeMachineUseResultPlugIn>(
                    view => view.ShowMachineUseResultAsync(machine!.Id, machineType, target),
                    true)
                .ConfigureAwait(false);
            foreach (var observer in map!.GetAttackablesInRange(target, 6).OfType<Player>())
            {
                await observer.InvokeViewPlugInAsync<ICastleSiegeMachineRegionNotifyPlugIn>(
                        view => view.ShowMachineRegionAsync(machineType, target))
                    .ConfigureAwait(false);
            }

            await this._delay.Invoke(ImpactDelay).ConfigureAwait(false);
            if (context.CurrentState != CastleSiegeState.Start)
            {
                return true;
            }

            foreach (var attackable in map.GetAttackablesInRange(target, 3).Where(attackable => attackable.IsAlive))
            {
                await attackable.AttackByAsync(player, null, false).ConfigureAwait(false);
            }

            return true;
        }
        finally
        {
            machine!.IsActive = false;
        }
    }
}
