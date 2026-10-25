// <copyright file="GameMasterEventActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Administration;

using System.Runtime.CompilerServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx;

/// <summary>Shares event start validation and cleanup between GM chat and administration.</summary>
public static class GameMasterEventActions
{
    private static readonly ConditionalWeakTable<IGameContext, AsyncLock> Gates = new();

    /// <summary>Requests a start of an enabled periodic mini game on this context.</summary>
    /// <param name="context">The owning game context.</param>
    /// <param name="type">The mini game type.</param>
    /// <param name="restart">Whether to dispose a running event first.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The confirmed request result.</returns>
    public static async ValueTask<GameServerAdminResult> StartAsync(IGameContext context, MiniGameType type, bool restart, CancellationToken cancellationToken)
    {
        using var gate = await Gates.GetValue(context, _ => new AsyncLock()).LockAsync(cancellationToken).ConfigureAwait(false);
        var starter = context.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(type);
        if (!Enum.IsDefined(type) || starter is null || !starter.IsEnabledOn(context))
        {
            return new(false, "This event is not active or is disabled on this game server. Check events_schedule.");
        }

        if (starter.IsEventActive(context) && !restart)
        {
            return new(false, "An event is already running. Use restart=true explicitly to remove its players and restart it.");
        }

        // The shared GM behavior also clears stale instances before requesting a new run.
        await starter.DisposeRunningGamesAsync(context).ConfigureAwait(false);
        starter.ForceStart();
        return GameServerAdminResult.Success;
    }
}
