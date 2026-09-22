// <copyright file="CastleSiegeMachineTalkPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.CastleSiege.NPC;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Opens a Castle Siege warfare machine for a player on the matching side.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegeMachineTalkPlugIn_Name), Description = nameof(PlugInResources.CastleSiegeMachineTalkPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("316BBCDB-1192-40CD-A159-7E8D1A25F449")]
public sealed class CastleSiegeMachineTalkPlugIn : IPlayerTalkToNpcPlugIn
{
    private readonly Func<Player, CastleSiegeContext?> _contextResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineTalkPlugIn"/> class.
    /// </summary>
    public CastleSiegeMachineTalkPlugIn()
        : this(CastleSiegeContextResolver.GetContext)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegeMachineTalkPlugIn"/> class.
    /// </summary>
    /// <param name="contextResolver">The Castle Siege context resolver.</param>
    internal CastleSiegeMachineTalkPlugIn(Func<Player, CastleSiegeContext?> contextResolver)
    {
        this._contextResolver = contextResolver;
    }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(
        Player player,
        NonPlayerCharacter npc,
        NpcTalkEventArgs eventArgs)
    {
        if (npc is not CastleSiegeMachine machine)
        {
            return;
        }

        eventArgs.HasBeenHandled = true;
        var context = this._contextResolver.Invoke(player);
        if (context is not { Configuration.Enabled: true }
            || context.CurrentState != CastleSiegeState.Start
            || !player.IsAlive
            || !ReferenceEquals(player.CurrentMap, machine.CurrentMap)
            || !player.IsInRange(machine.Position, CastleSiegeMachine.OperationRange))
        {
            await this.ShowMachineInterfaceAsync(player, machine, false).ConfigureAwait(false);
            return;
        }

        var isAvailable = false;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!machine.CanBeUsedBy(context.GetPlayerJoinSide(player))
                || (machine.Operator is { } currentOperator
                    && !ReferenceEquals(currentOperator, player)
                    && currentOperator.IsAlive
                    && ReferenceEquals(currentOperator.CurrentMap, machine.CurrentMap)))
            {
                isAvailable = false;
            }
            else
            {
                machine.Operator = player;
                isAvailable = true;
            }
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        await this.ShowMachineInterfaceAsync(player, machine, isAvailable).ConfigureAwait(false);
        if (!isAvailable)
        {
            return;
        }

        eventArgs.LeavesDialogOpen = true;
    }

    private ValueTask ShowMachineInterfaceAsync(Player player, CastleSiegeMachine machine, bool success)
    {
        return player.InvokeViewPlugInAsync<ICastleSiegeMachineInterfacePlugIn>(
            view => view.ShowMachineInterfaceAsync(success, machine.MachineType, machine.Id));
    }
}
