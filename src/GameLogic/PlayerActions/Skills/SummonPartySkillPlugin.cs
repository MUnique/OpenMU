// <copyright file="SummonPartySkillPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The summon skill action for Dark Lord.
/// </summary>
[PlugIn(nameof(SummonPartySkillPlugin), "Handles the summon party skill of the dark lord class.")]
[Guid("44e34497-c9e1-4c15-9388-589dfa3fa820")]
public class SummonPartySkillPlugin : TargetedSkillPluginBase
{
    private static readonly int CountdownSeconds = 5;

    /// <inheritdoc />
    public override short Key => 63;

    /// <inheritdoc />
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        var skillEntry = player.SkillList?.GetSkill(skillId);
        if (skillEntry?.Skill is null)
        {
            return;
        }

        if (player.Party == null)
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skillEntry.Skill).ConfigureAwait(false))
        {
            return;
        }

        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, player, skillEntry.Skill.Number, true), true).ConfigureAwait(false);

        var cancellationTokenSource = new SkillCancellationTokenSource();
        player.SkillCancelTokenSource = cancellationTokenSource;

        _ = this.RunSummonPartyAsync(player, cancellationTokenSource);
    }

    private async ValueTask RunSummonPartyAsync(Player player, CancellationTokenSource cancellationTokenSource)
    {
        var cancellationToken = cancellationTokenSource.Token;
        var targets = player.Party!.PartyList;
        var targetPlayers = targets.OfType<Player>().Where(p => p != player);

        try
        {
            for (var count = CountdownSeconds; count > 0; count--)
            {
                cancellationToken.ThrowIfCancellationRequested();

                foreach (var targetPlayer in targetPlayers)
                {
                    await targetPlayer.InvokeViewPlugInAsync<IChatViewPlugIn>(
                        p => p.ChatMessageAsync($"Summoning in {count} second(s)...", player.Name, ChatMessageType.Party)).ConfigureAwait(false);
                }

                if (!player.IsAlive || player.IsAtSafezone())
                {
                    await player.Party.SendChatMessageAsync("Summoning canceled.", player.Name).ConfigureAwait(false);
                    return;
                }

                targetPlayers = targetPlayers.Where(this.CanTargetBeTeleported);
            }

            await this.SummonTargetsAsync(player, targetPlayers).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // Handle cancellation (if needed)
            await player.Party.SendChatMessageAsync("Summoning canceled.", player.Name).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log unexpected exceptions
            player.Logger.LogWarning(ex, "Error during countdown");
        }
        finally
        {
            player.SkillCancelTokenSource?.Dispose();
            player.SkillCancelTokenSource = null;
        }
    }

    private async ValueTask SummonTargetsAsync(Player player, IEnumerable<Player> targetPlayers)
    {
        foreach (var targetPlayer in targetPlayers)
        {
            Point targetPoint = player.Position;
            bool foundValidPoint = false;
            int maxAttempts = 10;
            int attempts = 0;

            while (!foundValidPoint && attempts < maxAttempts)
            {
                attempts++;

                var offsetX = Rand.NextInt(-2, 3);
                var offsetY = Rand.NextInt(-2, 3);
                Point validPoint = new((byte)(player.Position.X + offsetX), (byte)(player.Position.Y + offsetY));

                if (player.CurrentMap!.Terrain.WalkMap[targetPoint.X, targetPoint.Y]
                    && player.Position.EuclideanDistanceTo(targetPoint) < 6)
                {
                    targetPoint = validPoint;
                    foundValidPoint = true;
                }
            }

            _ = Task.Run(() => targetPlayer.TeleportToMapAsync(player.CurrentMap!, targetPoint));
        }

        await player.ForEachWorldObserverAsync<INewPlayersInScopePlugIn>(p => p.NewPlayersInScopeAsync(targetPlayers, false), false).ConfigureAwait(false);
    }

    private bool CanTargetBeTeleported(Player target)
    {
        if (target.IsActive()
            && target.OpenedNpc == null
            && target.TradingPartner == null)
        {
            return true;
        }

        return false;
    }
}
