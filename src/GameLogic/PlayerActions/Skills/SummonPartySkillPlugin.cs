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

        if (skillEntry?.Skill is null || !this.CanPlayerSummon(player))
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

        _ = Task.Run(() => this.RunSummonPartyAsync(player, cancellationTokenSource));
    }

    private async ValueTask RunSummonPartyAsync(Player player, CancellationTokenSource cancellationTokenSource)
    {
        var cancellationToken = cancellationTokenSource.Token;
        var partyList = player.Party!.PartyList;
        var targetPlayers = partyList.OfType<Player>().Where(p => p != player).ToList();

        try
        {
            for (var count = CountdownSeconds; count > 0; count--)
            {
                cancellationToken.ThrowIfCancellationRequested();

                targetPlayers.RemoveAll(target => !this.CanPlayerSummonTarget(player, target));

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

                await Task.Delay(1000, cancellationToken).ConfigureAwait(false);
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
                Point testPoint = new((byte)(player.Position.X + offsetX), (byte)(player.Position.Y + offsetY));

                if (player.CurrentMap!.Terrain.WalkMap[testPoint.X, testPoint.Y]
                    && player.Position.EuclideanDistanceTo(targetPoint) < 6)
                {
                    targetPoint = testPoint;
                    foundValidPoint = true;
                }
            }

            if (player.CurrentMap!.Definition.TryGetRequirementError(targetPlayer, out var errorMessage))
            {
                await targetPlayer.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(errorMessage, Interfaces.MessageType.BlueNormal)).ConfigureAwait(false);
                continue;
            }

            await targetPlayer.TeleportToMapAsync(player.CurrentMap!, targetPoint).ConfigureAwait(false);
        }
    }

    private bool CanPlayerSummon(Player player)
    {
        return player.Party is not null
            && player.CurrentMiniGame is null;
    }

    private bool CanPlayerSummonTarget(Player player, Player target)
    {
        return target.IsActive()
            && target.Party?.Equals(player.Party) is true
            && target.OpenedNpc is null
            && target.TradingPartner is null
            && target.CurrentMiniGame is null;
    }
}
