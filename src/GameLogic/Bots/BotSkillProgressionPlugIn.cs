// <copyright file="BotSkillProgressionPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
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
            await this.SpendStatPointsAsync(player).ConfigureAwait(false);
            await this.LearnNewSkillsAsync(player).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogError(ex, "Failed to progress bot '{Name}' after level-up.", player.Name);
        }
    }

    private async ValueTask SpendStatPointsAsync(Player player)
    {
        var character = player.SelectedCharacter!;
        var points = character.LevelUpPoints;
        if (points <= 0)
        {
            return;
        }

        var weights = BotProgression.GetStatWeights(character.CharacterClass!);
        foreach (var (stat, amount) in BotProgression.SplitPoints(points, weights))
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
