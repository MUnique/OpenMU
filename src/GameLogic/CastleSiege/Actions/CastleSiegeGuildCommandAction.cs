// <copyright file="CastleSiegeGuildCommandAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege.Actions;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// Validates and delivers a directional guild command issued by a Castle Siege alliance master.
/// </summary>
public static class CastleSiegeGuildCommandAction
{
    /// <summary>
    /// Validates the requesting player and, if authorized, delivers the command to all same-side players
    /// currently on the Castle Siege map.
    /// </summary>
    /// <param name="player">The requesting player.</param>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="positionX">The target X coordinate.</param>
    /// <param name="positionY">The target Y coordinate.</param>
    /// <param name="command">The command type.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async ValueTask IssueCommandAsync(
        Player player,
        CastleSiegeContext? context,
        byte positionX,
        byte positionY,
        CastleSiegeCommandType command)
    {
        if (context is not { Configuration.Enabled: true, CurrentState: CastleSiegeState.Start })
        {
            return;
        }

        CastleSiegeJoinSide issuerSide;
        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            if (!context.GetSiegePlayers().Contains(player)
                || CastleSiegeGuildResolver.ResolveParticipatingAllianceMaster(player, context) is not { } participant)
            {
                return;
            }

            issuerSide = participant.Side;
        }
        finally
        {
            context.ExecutionLock.Release();
        }

        var recipients = context.GetSiegePlayers()
            .Where(candidate => context.GetPlayerJoinSide(candidate) == issuerSide)
            .ToList();

        await Task.WhenAll(recipients.Select(recipient =>
                recipient.InvokeViewPlugInAsync<ICastleSiegeCommandPlugIn>(
                        view => view.ShowGuildCommandAsync(issuerSide, positionX, positionY, command))
                    .AsTask()))
            .ConfigureAwait(false);
    }
}
