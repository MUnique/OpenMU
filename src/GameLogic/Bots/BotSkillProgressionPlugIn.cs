// <copyright file="BotSkillProgressionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Grows a server-side bot like a real player when it levels up during play: the earned stat points are
/// invested according to the bot's class build (see <see cref="BotProgression.GetStatWeights"/>), and any
/// class skill whose learn requirements (total energy, leadership, character level, ...) are now met is
/// learned - attack skills as well as the class's own buffs and heals. Skills are only ever learned for
/// the character's own class (<see cref="Skill.QualifiedCharacters"/>), using the same requirements the
/// game enforces for human players, so a grown bot matches a freshly generated one of the same level.
/// </summary>
[PlugIn]
[Display(Name = "Bot skill progression", Description = "Invests level-up stat points and teaches server-side bots new class- and level-appropriate skills as they level up.")]
[Guid("D1F4A7C2-6B3E-4A59-8E71-9C0D2F5B6A84")]
public class BotSkillProgressionPlugIn : ICharacterLevelUpPlugIn
{
    private static readonly IncreaseStatsAction IncreaseStatsAction = new();

    private static readonly BotSkillProgressionPlugIn CatchUp = new();

    /// <summary>
    /// Applies the progression a bot is owed but has not spent, when it enters the world. Points are
    /// otherwise only invested on a level-up, so a character which was given points while it was not
    /// playing - a freshly generated one, or one whose level-up handler failed - would carry them around
    /// unspent until its next level-up and fight with the strength of a much weaker character in the
    /// meantime. Cheap: only a bot which actually holds points is progressed.
    /// </summary>
    /// <param name="player">The bot which entered the world.</param>
    public static void CatchUpPendingProgress(Player player)
    {
        if (player.SelectedCharacter?.LevelUpPoints > 0)
        {
            CatchUp.CharacterLeveledUp(player);
        }
    }

    /// <inheritdoc />
    public void CharacterLeveledUp(Player player)
    {
        if (player.Account?.IsBot != true
            || player.SelectedCharacter?.CharacterClass is not { }
            || player.SkillList is not { })
        {
            return;
        }

        // Queue the progression into the bot's AI tick instead of running it right here: this hook fires
        // from the experience/level-up path while the combat handler may be enumerating the skill list on
        // its own timer - the tick serializes both. No own SaveChanges here - the new stats and skills
        // are persisted by the periodic save, avoiding extra concurrency pressure.
        if (player is Offline.OfflinePlayer offlinePlayer)
        {
            offlinePlayer.PendingBotActions.Enqueue(() => new ValueTask(this.ProgressAsync(offlinePlayer)));
        }
        else
        {
            _ = this.ProgressAsync(player);
        }
    }

    private async Task ProgressAsync(Player player)
    {
        try
        {
            this.EvolveClassIfDue(player);
            await this.SpendStatPointsAsync(player).ConfigureAwait(false);
            await this.LearnNewSkillsAsync(player).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Failed to progress bot '{Name}' after level-up.", player.Name);
        }
    }

    /// <summary>
    /// Changes the bot into its second-generation class (Dark Knight -> Blade Knight etc.) once it
    /// reaches <see cref="BotProgression.ClassEvolutionLevel"/> - the exact assignment the class-change
    /// quest performs for a human player (see <c>QuestCompletionAction</c>); simulating the quest run
    /// itself would be invisible to observers anyway. Skills and gear of the new class follow
    /// automatically, because all bot progression keys off the current class's qualifications.
    /// </summary>
    private void EvolveClassIfDue(Player player)
    {
        var character = player.SelectedCharacter!;
        if (player.Level < BotProgression.ClassEvolutionLevel
            || BotProgression.GetEvolutionTarget(character.CharacterClass!) is not { } evolvedClass)
        {
            return;
        }

        character.CharacterClass = evolvedClass;
        player.Logger.LogInformation(
            "Bot '{Name}' evolved into {Class} at level {Level}.",
            player.Name,
            evolvedClass.Name,
            player.Level);
    }

    private async ValueTask SpendStatPointsAsync(Player player)
    {
        var character = player.SelectedCharacter!;
        var characterClass = character.CharacterClass!;
        var points = character.LevelUpPoints;
        if (points <= 0)
        {
            return;
        }

        var resetMeta = BotResetHandler.GetResetConfiguration(player.GameContext) is not null;
        var weights = BotProgression.GetStatWeights(characterClass, character.Name, resetMeta);
        var vitalityTarget = resetMeta ? BotProgression.GetVitalityTarget(character.Name) : (int?)null;

        // Mirrors the capacity checks of the stat-increase action (a stat's configured maximum on fun
        // servers), plus the bot's personal vitality target on reset-meta servers: a full stat drops
        // out of the split, so its share flows into the rest of the build instead of getting lost.
        long CapacityOf(AttributeDefinition stat)
        {
            var classBase = characterClass.StatAttributes.FirstOrDefault(a => a.Attribute == stat);
            var current = (long)(player.Attributes?[stat] ?? 0f);
            var capacity = long.MaxValue;
            if (classBase?.Attribute?.MaximumValue is { } maximumValue)
            {
                capacity = (long)maximumValue - current;
            }

            if (vitalityTarget is { } target && stat == Stats.BaseVitality)
            {
                var invested = current - (long)(classBase?.BaseValue ?? 0f);
                capacity = Math.Min(capacity, target - invested);
            }

            return capacity;
        }

        foreach (var (stat, amount) in BotProgression.SplitPoints(points, weights, CapacityOf))
        {
            if (amount > 0)
            {
                await IncreaseStatsAction.IncreaseStatsAsync(player, stat, (ushort)amount).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask LearnNewSkillsAsync(Player player)
    {
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var skillList = player.SkillList!;

        float? GetValue(AttributeDefinition attribute) => player.Attributes?[attribute];

        foreach (var skill in player.GameContext.Configuration.Skills)
        {
            if (!BotProgression.IsBotLearnableSkill(skill)
                || !skill.QualifiedCharacters.Contains(characterClass)
                || skillList.ContainsSkill((ushort)skill.Number)
                || !BotProgression.MeetsRequirements(skill, GetValue))
            {
                continue;
            }

            await skillList.AddLearnedSkillAsync(skill).ConfigureAwait(false);
            player.Logger.LogInformation("Bot '{Name}' learned '{Skill}' at level {Level}.", player.Name, skill.Name, player.Level);
        }
    }
}
