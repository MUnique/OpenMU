// <copyright file="PlayerLosesExperienceAfterDeathPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin decreases the experience after the player has been killed by a monster.
/// </summary>
[PlugIn("Player exp loss after death", "This plugin decreases the experience after the player has been killed by a monster.")]
[Guid("CA9EACAC-1BD4-44BA-9187-C9F2CEF4E254")]
public class PlayerLosesExperienceAfterDeathPlugIn : IAttackableGotKilledPlugIn, ISupportCustomConfiguration<PlayerLosesExperienceAfterDeathPlugInConfiguration>, ISupportDefaultCustomConfiguration
{
    /// <inheritdoc/>
    public PlayerLosesExperienceAfterDeathPlugInConfiguration? Configuration { get; set; }

    /// <summary>
    /// Is called when an <see cref="IAttackable" /> object got killed by another.
    /// </summary>
    /// <param name="killed">The killed <see cref="IAttackable" />.</param>
    /// <param name="killer">The killer.</param>
    public async ValueTask AttackableGotKilledAsync(IAttackable killed, IAttacker? killer)
    {
        if (killer is not Monster || killed is not Player player
                                  || player.SelectedCharacter is not { } selectedCharacter
                                  || selectedCharacter.CharacterClass is not { } characterClass
                                  || player.Attributes is not { } attributes)
        {
            return;
        }

        if (player.CurrentMiniGame is not null)
        {
            return;
        }

        const int MAP_INDEX_CRYWOLF_FIRSTZONE = 34;
        if (player.CurrentMap?.Definition.Number == MAP_INDEX_CRYWOLF_FIRSTZONE)
        {
            return;
        }

        this.Configuration ??= CreateDefaultConfiguration();
        if (characterClass.IsMasterClass)
        {
            if (this.CalculateNewExperience(selectedCharacter, selectedCharacter.MasterExperience, player.GameContext.MasterExperienceTable, attributes, Stats.MasterLevel, player.GameContext.Configuration.MaximumMasterLevel) is { } newExperience)
            {
                selectedCharacter.MasterExperience = newExperience;
            }
        }
        else
        {
            if (this.CalculateNewExperience(selectedCharacter, selectedCharacter.Experience, player.GameContext.ExperienceTable, attributes, Stats.Level, player.GameContext.Configuration.MaximumLevel) is { } newExperience)
            {
                selectedCharacter.Experience = newExperience;
            }
        }
    }

    /// <inheritdoc/>
    public object CreateDefaultConfig()
    {
        return CreateDefaultConfiguration();
    }

    private static PlayerLosesExperienceAfterDeathPlugInConfiguration CreateDefaultConfiguration()
    {
        return new()
        {
            LossesPerLevel =
            [
                new(
                    10,
                    false,
                    [
                        new(HeroState.New, 1),
                        new(HeroState.Hero, 1),
                        new(HeroState.LightHero, 1),
                        new(HeroState.Normal, 1),
                        new(HeroState.PlayerKillWarning, 5),
                        new(HeroState.PlayerKiller1stStage, 6),
                        new(HeroState.PlayerKiller2ndStage, 7),
                    ]),
                new(
                    150,
                    false,
                    [
                        new(HeroState.New, 1),
                        new(HeroState.Hero, 1),
                        new(HeroState.LightHero, 1),
                        new(HeroState.Normal, 1),
                        new(HeroState.PlayerKillWarning, 4),
                        new(HeroState.PlayerKiller1stStage, 5),
                        new(HeroState.PlayerKiller2ndStage, 6),
                    ]),
                new(
                    220,
                    false,
                    [
                        new(HeroState.New, 1),
                        new(HeroState.Hero, 1),
                        new(HeroState.LightHero, 1),
                        new(HeroState.Normal, 1),
                        new(HeroState.PlayerKillWarning, 3),
                        new(HeroState.PlayerKiller1stStage, 4),
                        new(HeroState.PlayerKiller2ndStage, 5),
                    ]),
                new(
                    0,
                    true,
                    [
                        new(HeroState.New, 0.7),
                        new(HeroState.Hero, 0.7),
                        new(HeroState.LightHero, 0.7),
                        new(HeroState.Normal, 0.7),
                        new(HeroState.PlayerKillWarning, 2),
                        new(HeroState.PlayerKiller1stStage, 3),
                        new(HeroState.PlayerKiller2ndStage, 4),
                    ]),
            ],
        };
    }

    private long? CalculateNewExperience(Character character, long currentExperience, long[] expTable, IAttributeSystem attributes, AttributeDefinition levelDefinition, int maximumLevel)
    {
        var level = (int)attributes[levelDefinition];
        if (level >= maximumLevel)
        {
            return null;
        }

        var expForCurrentLevel = expTable[level];
        var expForNextLevel = expTable[level + 1];
        var expDiff = expForNextLevel - expForCurrentLevel;
        var lossPercentage = this.GetLossPercentage(level, levelDefinition == Stats.MasterLevel, character.State);
        var loss = (int)(expDiff * (lossPercentage / 100.0));
        return Math.Max(expForCurrentLevel, currentExperience - loss);
    }

    private double GetLossPercentage(int playerLevel, bool isMaster, HeroState heroState)
    {
        return this.Configuration?.LossesPerLevel
            .Where(l => l.IsMaster == isMaster)
            .OrderByDescending(l => l.MinimumLevel)
            .FirstOrDefault(l => l.MinimumLevel <= playerLevel)
            ?.Losses.FirstOrDefault(loss => loss.State == heroState)
            ?.ExperienceLossPercentage ?? 0;
    }
}