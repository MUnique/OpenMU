// <copyright file="CastleSiegeGuildCommandAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using System.Runtime.CompilerServices;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and delivers a directional guild command issued by a Castle Siege alliance master.
/// </summary>
public static class CastleSiegeGuildCommandAction
{
    /// <summary>
    /// The minimum time between two commands accepted from the same issuer. Each accepted command fans out to
    /// every same-side player on the map and takes <see cref="CastleSiegeContext.ExecutionLock"/> along the
    /// way, so a cheap cooldown here removes an amplification vector without costing legitimate use - the
    /// client's own marker lifetime (100 game ticks) makes rapid re-issues pointless anyway.
    /// </summary>
    private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(1);

    private static readonly ConditionalWeakTable<Player, StrongBox<long>> NextCommandTicks = new();

    /// <summary>
    /// Gets the number of command marker slots in the Season 6 client.
    /// Larger indexes would address outside its buffer.
    /// </summary>
    public static byte CommandGroupCount => 7;

    /// <summary>
    /// Validates the requesting player and, if authorized, delivers the command to all same-side players
    /// currently on the Castle Siege map.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="team">
    /// The client's command-group (squad) slot, 0-6. Relayed unchanged - it selects which of the client's
    /// seven mini-map marker slots the order is drawn into, it does not select an audience.
    /// </param>
    /// <param name="positionX">The target X coordinate.</param>
    /// <param name="positionY">The target Y coordinate.</param>
    /// <param name="command">The command type.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async ValueTask IssueCommandAsync(
        Player player,
        CastleSiegeContext? context,
        byte team,
        byte positionX,
        byte positionY,
        CastleSiegeCommandType command)
    {
        if (context is not { Configuration.Enabled: true }
            || team >= CommandGroupCount
            || command is not (CastleSiegeCommandType.Attack or CastleSiegeCommandType.Defend or CastleSiegeCommandType.Wait)
            || !TryTakeCommandSlot(player, context.TimeProvider.GetUtcNow().UtcTicks))
        {
            return;
        }

        IReadOnlyList<Player> recipients;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (context.CurrentState != CastleSiegeState.Start
                || !context.GetSiegePlayers().Contains(player)
                || CastleSiegeGuildResolver.ResolveParticipatingAllianceMaster(player, context) is null)
            {
                return;
            }

            // Resolved the same way the recipient filter below resolves it, so the issuer can't briefly
            // diverge from the audience it's about to be compared against (e.g. right after a crown capture
            // swaps guild.Side but before the per-character PlayerJoinSides resync has run).
            var issuerSide = context.GetPlayerJoinSide(player);
            if (issuerSide == CastleSiegeJoinSide.None)
            {
                return;
            }

            recipients = context.GetSiegePlayers()
                .Where(candidate => context.GetPlayerJoinSide(candidate) == issuerSide)
                .ToList();
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        await Task.WhenAll(recipients.Select(recipient =>
                recipient.InvokeViewPlugInAsync<ICastleSiegeCommandPlugIn>(
                        view => view.ShowGuildCommandAsync(team, positionX, positionY, command))
                    .AsTask()))
            .ConfigureAwait(false);
    }

    private static bool TryTakeCommandSlot(Player player, long utcTicks)
    {
        // Stamp attempts before waiting for the siege lock, including unauthorized attempts. Atomic updates
        // also prevent simultaneous packets from the same player from passing the gate together.
        var nextCommand = NextCommandTicks.GetOrCreateValue(player);
        var nextTicks = Volatile.Read(ref nextCommand.Value);
        while (utcTicks >= nextTicks)
        {
            var observedTicks = Interlocked.CompareExchange(ref nextCommand.Value, utcTicks + Cooldown.Ticks, nextTicks);
            if (observedTicks == nextTicks)
            {
                return true;
            }

            nextTicks = observedTicks;
        }

        return false;
    }
}
