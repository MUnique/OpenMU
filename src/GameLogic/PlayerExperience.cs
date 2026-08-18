// <copyright file="PlayerExperience.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.GameLogic.Views.World;
using Nito.AsyncEx;

/// <summary>
/// The experience and the leveling of a <see cref="Player"/>.
/// </summary>
internal sealed class PlayerExperience
{
    private readonly Player _player;

    private readonly AsyncLock _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerExperience"/> class.
    /// </summary>
    /// <param name="player">The player.</param>
    public PlayerExperience(Player player)
    {
        this._player = player;
    }

    /// <summary>
    /// Adds experience points after killing the target object.
    /// </summary>
    /// <param name="killedObject">The killed object.</param>
    /// <returns>The gained experience.</returns>
    public async ValueTask<int> AddAfterKillAsync(IAttackable killedObject)
    {
        if (!this.TryGetExperienceKind(out var isMasterExperience))
        {
            return 0;
        }

        var experience = await this.CalculateAfterKillAsync(killedObject).ConfigureAwait(false);
        if (experience == 0)
        {
            return 0;
        }

        if (isMasterExperience)
        {
            await this.AddMasterExperienceAsync(experience, killedObject).ConfigureAwait(false);
        }
        else
        {
            await this.AddExperienceAsync(experience, killedObject).ConfigureAwait(false);
        }

        if (this._player.GameContext.PlugInManager.GetPlugInPoint<IPlayerGainedExperiencePlugIn>() is { } plugInPoint)
        {
            await plugInPoint.PlayerGainedExperienceAsync(this._player, experience, killedObject, isMasterExperience).ConfigureAwait(false);
        }

        return experience;
    }

    /// <summary>
    /// Calculates the amount of experience gained after a kill, without applying it to the character.
    /// </summary>
    /// <param name="killedObject">The killed monster.</param>
    /// <returns>The calculated experience amount.</returns>
    public async ValueTask<int> CalculateAfterKillAsync(IAttackable killedObject)
    {
        if (!this.TryGetExperienceKind(out var isMasterExperience)
            || this._player.Attributes is not { } attributes)
        {
            return 0;
        }

        var expRateAttribute = isMasterExperience ? Stats.MasterExperienceRate : Stats.ExperienceRate;
        var gameRate = isMasterExperience ? this._player.GameContext.MasterExperienceRate : this._player.GameContext.ExperienceRate;

        var experience = killedObject.CalculateBaseExperience(attributes[Stats.TotalLevel]);
        experience *= gameRate;
        experience *= attributes[expRateAttribute] + attributes[Stats.BonusExperienceRate];
        experience *= this._player.CurrentMap?.Definition.ExpMultiplier ?? 1;

        if (this._player.GameContext.PlugInManager.GetPlugInPoint<IExperienceCalculationPlugIn>() is { } plugInPoint)
        {
            var args = new ExperienceCalculationArgs(killedObject, isMasterExperience, experience);
            await plugInPoint.CalculateExperienceAsync(this._player, args).ConfigureAwait(false);
            experience = args.Experience;
        }

        var minMultiplier = attributes[Stats.RandomExperienceMinMultiplier];
        var maxMultiplier = attributes[Stats.RandomExperienceMaxMultiplier];
        if (minMultiplier > 0 && maxMultiplier > 0)
        {
            var minimumExperience = (int)(experience * minMultiplier);
            var maximumExperience = (int)(experience * maxMultiplier);
            if (minimumExperience < maximumExperience)
            {
                return Rand.NextInt(minimumExperience, maximumExperience);
            }
        }

        return (int)experience;
    }

    /// <summary>
    /// Adds the master experience to the current character.
    /// </summary>
    /// <param name="experience">The experience that should be added.</param>
    /// <param name="killedObject">The killed object that caused the experience gain.</param>
    public async ValueTask AddMasterExperienceAsync(int experience, IAttackable? killedObject)
    {
        using var d = await this._lock.LockAsync().ConfigureAwait(false);
        await this.AddMasterExperienceCoreAsync(experience, killedObject).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the experience to the current character.
    /// </summary>
    /// <param name="experience">The experience that should be added.</param>
    /// <param name="killedObject">The killed object that caused the experience gain.</param>
    public async ValueTask AddExperienceAsync(int experience, IAttackable? killedObject)
    {
        using var d = await this._lock.LockAsync().ConfigureAwait(false);
        await this.AddExperienceCoreAsync(experience, killedObject).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines whether the player gains master experience instead of normal experience.
    /// </summary>
    /// <param name="isMasterExperience">If set to <c>true</c>, master experience is gained.</param>
    /// <returns><c>True</c>, if the player can gain experience at all; Otherwise, <c>false</c>.</returns>
    private bool TryGetExperienceKind(out bool isMasterExperience)
    {
        isMasterExperience = false;
        if (this._player.SelectedCharacter?.CharacterClass is not { } characterClass
            || this._player.Attributes is not { } attributes)
        {
            return false;
        }

        var currentLevel = (short)attributes[Stats.Level];
        var isMaxLevel = currentLevel == this._player.GameContext.Configuration.MaximumLevel;
        isMasterExperience = characterClass.IsMasterClass && isMaxLevel;
        return true;
    }

    private async ValueTask AddMasterExperienceCoreAsync(int experience, IAttackable? killedObject)
    {
        var player = this._player;
        if (player.Attributes![Stats.MasterLevel] >= player.GameContext.Configuration.MaximumMasterLevel)
        {
            await player.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync(0, killedObject, ExperienceType.MaxMasterLevelReached)).ConfigureAwait(false);
            return;
        }

        if (killedObject is not null && killedObject.Attributes[Stats.Level] < player.GameContext.Configuration.MinimumMonsterLevelForMasterExperience)
        {
            await player.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync(0, killedObject, ExperienceType.MonsterLevelTooLowForMasterExperience)).ConfigureAwait(false);
            return;
        }

        long exp = experience;

        bool lvlup = false;
        var expTable = player.GameContext.MasterExperienceTable;
        if (expTable[(int)player.Attributes[Stats.MasterLevel] + 1] - player.SelectedCharacter!.MasterExperience < exp)
        {
            exp = expTable[(int)player.Attributes[Stats.MasterLevel] + 1] - player.SelectedCharacter.MasterExperience;
            lvlup = true;
        }

        player.SelectedCharacter.MasterExperience += exp;

        await player.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync((int)exp, killedObject, ExperienceType.Master)).ConfigureAwait(false);

        if (lvlup)
        {
            player.Attributes[Stats.MasterLevel]++;
            player.SelectedCharacter.MasterLevelUpPoints += (int)player.Attributes[Stats.MasterPointsPerLevelUp];
            player.SetReclaimableAttributesToMaximum();
            player.Logger.LogDebug("Character {0} leveled up to master level {1}", player.SelectedCharacter.Name, player.Attributes[Stats.MasterLevel]);

            if (player.GameContext.PlugInManager.GetPlugInPoint<ICharacterMasterLevelUpPlugIn>() is { } plugInPoint)
            {
                await plugInPoint.CharacterMasterLeveledUpAsync(player).ConfigureAwait(false);
            }

            await player.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateMasterLevelAsync()).ConfigureAwait(false);
            await player.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(player, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);
        }
    }

    private async ValueTask AddExperienceCoreAsync(int experience, IAttackable? killedObject)
    {
        var player = this._player;
        var remainingExperience = experience;
        while (remainingExperience > 0)
        {
            if (player.Attributes![Stats.Level] >= player.GameContext.Configuration.MaximumLevel)
            {
                await player.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync(0, killedObject, ExperienceType.MaxLevelReached)).ConfigureAwait(false);
                return;
            }

            long gainedExperience = remainingExperience;
            bool isLevelUp = false;
            var expTable = player.GameContext.ExperienceTable;
            var expForNextLevel = expTable[(int)player.Attributes[Stats.Level] + 1];
            if (expForNextLevel - player.SelectedCharacter!.Experience < gainedExperience)
            {
                gainedExperience = expForNextLevel - player.SelectedCharacter.Experience;
                isLevelUp = true;
            }

            player.SelectedCharacter.Experience += gainedExperience;

            await player.InvokeViewPlugInAsync<IAddExperiencePlugIn>(p => p.AddExperienceAsync((int)gainedExperience, killedObject, ExperienceType.Normal)).ConfigureAwait(false);

            if (!isLevelUp)
            {
                return;
            }

            player.Attributes[Stats.Level]++;
            player.SelectedCharacter.LevelUpPoints += (int)player.Attributes[Stats.PointsPerLevelUp];
            player.SetReclaimableAttributesToMaximum();
            player.Logger.LogDebug("Character {0} leveled up to {1}", player.SelectedCharacter.Name, player.Attributes[Stats.Level]);

            player.GameContext.PlugInManager.GetPlugInPoint<ICharacterLevelUpPlugIn>()?.CharacterLeveledUp(player);

            await player.InvokeViewPlugInAsync<IUpdateLevelPlugIn>(p => p.UpdateLevelAsync()).ConfigureAwait(false);
            await player.ForEachWorldObserverAsync<IShowEffectPlugIn>(p => p.ShowEffectAsync(player, IShowEffectPlugIn.EffectType.LevelUp), true).ConfigureAwait(false);

            remainingExperience -= (int)gainedExperience;
            if (remainingExperience <= 0
                || player.Attributes[Stats.Level] >= player.GameContext.Configuration.MaximumLevel
                || player.GameContext.Configuration.PreventExperienceOverflow)
            {
                return;
            }
        }
    }
}
