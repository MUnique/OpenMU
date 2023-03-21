// <copyright file="NovaSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Threading;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// The nova skill action.
/// </summary>
public class NovaSkillAction
{
    private const byte NovaSkillId = 40;
    private const byte NovaStartId = 58;
    private static readonly TimeSpan NovaStepDelay = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// The nova damage per stage, which is still hardcoded. May be configurable later.
    /// </summary>
    private static readonly int[] NovaDamageTable = { 0, 20, 50, 99, 160, 225, 325, 425, 550, 700, 880, 1090, 1320 };

    /// <summary>
    /// Starts the nova skill.
    /// </summary>
    /// <param name="player">The player which performs the skill.</param>
    public async ValueTask StartNovaSkillAsync(Player player)
    {
        if (player.NovaCancellationTokenSource is not null)
        {
            // A nova is already ongoing.
            return;
        }

        var skillEntry = player.SkillList?.GetSkill(NovaSkillId);
        if (skillEntry?.Skill is null)
        {
            return;
        }

        // Consume full ability points first...
        var novaStart = player.GameContext.Configuration.Skills.First(s => s.Number == NovaStartId);
        if (!await player.TryConsumeForSkillAsync(novaStart).ConfigureAwait(false))
        {
            return;
        }

        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowNovaStartAsync(player), true).ConfigureAwait(false);
        var cancellationTokenSource = new NovaCancellationTokenSource();
        player.NovaCancellationTokenSource = cancellationTokenSource;

        _ = this.RunNovaAsync(player, skillEntry, cancellationTokenSource);
    }

    /// <summary>
    /// Stops the nova skill.
    /// </summary>
    /// <param name="player">The player which performed the skill.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    public async ValueTask StopNovaSkillAsync(Player player, ushort extraTargetId)
    {
        player.NovaCancellationTokenSource?.CancelWithExtraTarget(extraTargetId);
    }

    private async ValueTask RunNovaAsync(Player player, SkillEntry skillEntry, NovaCancellationTokenSource cancellationTokenSource)
    {
        var cancellationToken = cancellationTokenSource.Token;
        if (player.Attributes is not { } playerAttributes
            || skillEntry.Skill is not { } skill)
        {
            return;
        }

        byte completedSteps = 0;

        var stepDamageElement = new SimpleElement(0, AggregateType.AddRaw);
        playerAttributes.AddElement(stepDamageElement, Stats.NovaStageDamage);
        try
        {
            try
            {
                while (completedSteps < NovaDamageTable.Length - 1)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
                    {
                        break;
                    }

                    completedSteps++;
                    stepDamageElement.Value = NovaDamageTable[completedSteps];
                    var steps = completedSteps;
                    await player.ForEachWorldObserverAsync<IShowSkillStageUpdatePlugIn>(p => p.UpdateSkillStageAsync(player, NovaSkillId, steps), true).ConfigureAwait(false);
                    await Task.Delay(NovaStepDelay, cancellationToken).ConfigureAwait(false); // Hint: Player could cancel the nova 500 ms before end without damage loss - if he is good
                }
            }
            catch (OperationCanceledException)
            {
                // This is expected when the player stopped nova before.
            }

            await this.AttackTargetsAsync(player, skillEntry, cancellationTokenSource).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Unexpected error during performing nova skill");
            player.NovaCancellationTokenSource?.Dispose();
            player.NovaCancellationTokenSource = null;
        }
        finally
        {
            player.Attributes?.RemoveElement(stepDamageElement, Stats.NovaStageDamage);
        }
    }

    private async ValueTask AttackTargetsAsync(Player player, SkillEntry skillEntry, NovaCancellationTokenSource cancellationTokenSource)
    {
        if (!player.IsAlive || player.IsAtSafezone() || skillEntry.Skill is not { } skill)
        {
            return;
        }

        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, player, skill.Number, true), true).ConfigureAwait(false);
        var explicitTargetId = cancellationTokenSource.ExplicitTargetId;
        var targets = this.DetermineTargets(player, skill, explicitTargetId);

        // Set cancellation token source to null, so that the next nova can be started.
        player.NovaCancellationTokenSource?.Dispose();
        player.NovaCancellationTokenSource = null;

        await Task.Delay(500, CancellationToken.None).ConfigureAwait(false);

        foreach (var target in targets)
        {
            await target.AttackByAsync(player, skillEntry, false).ConfigureAwait(false);
        }
    }

    private IEnumerable<IAttackable> DetermineTargets(Player player, Skill skill, ushort? explicitTargetId)
    {
        bool FilterPlayer(Player attackable)
        {
            if (attackable == player)
            {
                // Don't attack yourself
                return false;
            }

            if (attackable.Id == explicitTargetId)
            {
                return true;
            }

            if (player.GuildWarContext is { } attackerContext
                && attackable.GuildWarContext is { } defenderContext
                && attackerContext.Team != defenderContext.Team
                && attackerContext.Score == defenderContext.Score)
            {
                return true;
            }

            // todo: during castle siege, nova attacks everyone
            // todo: during duel, it always attacks the opponent
            return false;
        }

        bool FilterMonster(Monster monster)
        {
            if (monster.Id == explicitTargetId)
            {
                return true;
            }

            if (monster.SummonedBy is not { } summonedBy)
            {
                // attack all monsters which are not summoned by a player.
                return true;
            }

            return FilterPlayer(summonedBy);
        }

        var targets = player.CurrentMap!
            .GetAttackablesInRange(player.Position, skill.Range)
            .Where(a => a.IsAlive)
            .Where(a => a is not Monster || FilterMonster((Monster)a))
            .Where(a => a is not Player || FilterPlayer((Player)a));

        return targets;
    }
}