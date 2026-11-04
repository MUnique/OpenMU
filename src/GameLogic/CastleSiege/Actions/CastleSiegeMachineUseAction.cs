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
    // The landing notification reaches players beyond the area which receives damage.
    private const int ImpactNotificationRange = 6;
    private const int ImpactDamageRange = 3;

    // Keep the machine reserved until its client-visible projectile reaches the target.
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
            await ShowMachineUseResultAsync(player, false, machineId, default, default).ConfigureAwait(false);
            return false;
        }

        CastleSiegeMachine? machine = null;
        GameMap? map = null;
        CastleSiegeMachineType machineType = default;
        Point target = default;
        var isValidRequest = false;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            var playerMap = player.CurrentMap;
            var foundMachine = playerMap?.GetObject(machineId) as CastleSiegeMachine;
            machineType = foundMachine?.MachineType ?? default;
            if (context.CurrentState == CastleSiegeState.Start
                && player.IsAlive
                && foundMachine is not null
                && ReferenceEquals(player.OpenedNpc, foundMachine)
                && player.IsInRange(foundMachine.Position, CastleSiegeMachine.OperationRange)
                && ReferenceEquals(foundMachine.Operator, player)
                && foundMachine.CanBeUsedBy(context.GetPlayerJoinSide(player))
                && !foundMachine.IsActive
                && targetZoneIndex != 0)
            {
                var zones = foundMachine.MachineType == CastleSiegeMachineType.Attack
                    ? context.Configuration.AttackMachineZones
                    : context.Configuration.DefenseMachineZones;
                if (targetZoneIndex <= zones.Count)
                {
                    var targetZone = zones.ElementAt(targetZoneIndex - 1);
                    if (targetZone.X1 <= targetZone.X2 && targetZone.Y1 <= targetZone.Y2)
                    {
                        machine = foundMachine;
                        map = playerMap;
                        machineType = foundMachine.MachineType;
                        target = new Point(
                            checked((byte)this._randomizer.NextInt(targetZone.X1, targetZone.X2 + 1)),
                            checked((byte)this._randomizer.NextInt(targetZone.Y1, targetZone.Y2 + 1)));
                        machine.IsActive = true;
                        isValidRequest = true;
                    }
                }
            }
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        if (!isValidRequest)
        {
            await ShowMachineUseResultAsync(player, false, machineId, machineType, default).ConfigureAwait(false);
            return false;
        }

        try
        {
            await player.ForEachWorldObserverAsync<ICastleSiegeMachineUseResultPlugIn>(
                    view => view.ShowMachineUseResultAsync(true, machine!.Id, machineType, target),
                    true)
                .ConfigureAwait(false);
            foreach (var observer in map!.GetAttackablesInRange(target, ImpactNotificationRange).OfType<Player>())
            {
                await observer.InvokeViewPlugInAsync<ICastleSiegeMachineRegionNotifyPlugIn>(
                        view => view.ShowMachineRegionAsync(machineType, target))
                    .ConfigureAwait(false);
            }

            // Don't wait for completion: this detached task releases the machine after the delayed impact.
            _ = this.ApplyImpactAsync(context, player, machine!, map!, target);
            return true;
        }
        catch
        {
            machine!.IsActive = false;
            throw;
        }
    }

    private static bool IsValidImpactTarget(
        CastleSiegeContext context,
        CastleSiegeMachine machine,
        Player player,
        IAttackable attackable)
    {
        return !ReferenceEquals(attackable, player)
               && attackable.IsActive()
               && !attackable.IsAtSafezone()
               && !IsFriendlyTarget(context, machine, attackable);
    }

    private static bool IsFriendlyTarget(
        CastleSiegeContext context,
        CastleSiegeMachine machine,
        IAttackable attackable)
    {
        // Configured structures retain their assigned side in the NPC definition across the battle.
        return attackable switch
        {
            Player targetPlayer => machine.IsSameSide(context.GetPlayerJoinSide(targetPlayer)),
            ICastleSiegeNpc targetNpc => machine.IsSameSide(targetNpc.Runtime.Definition.DefaultSide),
            _ => false,
        };
    }

    private static ValueTask ShowMachineUseResultAsync(
        Player player,
        bool success,
        ushort machineId,
        CastleSiegeMachineType machineType,
        Point target)
    {
        return player.InvokeViewPlugInAsync<ICastleSiegeMachineUseResultPlugIn>(
            view => view.ShowMachineUseResultAsync(success, machineId, machineType, target));
    }

    private async Task ApplyImpactAsync(
        CastleSiegeContext context,
        Player player,
        CastleSiegeMachine machine,
        GameMap map,
        Point target)
    {
        try
        {
            await this._delay.Invoke(ImpactDelay).ConfigureAwait(false);
            if (context.CurrentState != CastleSiegeState.Start)
            {
                return;
            }

            foreach (var attackable in map.GetAttackablesInRange(target, ImpactDamageRange)
                         .Where(attackable => IsValidImpactTarget(context, machine, player, attackable)))
            {
                // Machines are passive NPCs, so their damage deliberately uses the operator and the regular player damage pipeline.
                await attackable.AttackByAsync(player, null, false).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Castle Siege warfare-machine impact failed.");
        }
        finally
        {
            machine.IsActive = false;
        }
    }
}
