// <copyright file="IllusionTempleInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// The initializer for the illusion temple event.
/// </summary>
internal class IllusionTempleInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IllusionTempleInitializer" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IllusionTempleInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var illusionTemple1 = this.CreateIllusionTempleDefinition(1, IllusionTemple1.Number, 3000000);
        illusionTemple1.MinimumCharacterLevel = 220;
        illusionTemple1.MaximumCharacterLevel = 270;
        illusionTemple1.MinimumSpecialCharacterLevel = 220;
        illusionTemple1.MaximumSpecialCharacterLevel = 270;

        var illusionTemple2 = this.CreateIllusionTempleDefinition(2, IllusionTemple2.Number, 4000000);
        illusionTemple2.MinimumCharacterLevel = 271;
        illusionTemple2.MaximumCharacterLevel = 320;
        illusionTemple2.MinimumSpecialCharacterLevel = 271;
        illusionTemple2.MaximumSpecialCharacterLevel = 320;

        var illusionTemple3 = this.CreateIllusionTempleDefinition(3, IllusionTemple3.Number, 5000000);
        illusionTemple3.MinimumCharacterLevel = 321;
        illusionTemple3.MaximumCharacterLevel = 350;
        illusionTemple3.MinimumSpecialCharacterLevel = 321;
        illusionTemple3.MaximumSpecialCharacterLevel = 350;

        var illusionTemple4 = this.CreateIllusionTempleDefinition(4, IllusionTemple4.Number, 6000000);
        illusionTemple4.MinimumCharacterLevel = 351;
        illusionTemple4.MaximumCharacterLevel = 380;
        illusionTemple4.MinimumSpecialCharacterLevel = 351;
        illusionTemple4.MaximumSpecialCharacterLevel = 380;

        var illusionTemple5 = this.CreateIllusionTempleDefinition(5, IllusionTemple5.Number, 7000000);
        illusionTemple5.MinimumCharacterLevel = 381;
        illusionTemple5.MaximumCharacterLevel = 399;
        illusionTemple5.MinimumSpecialCharacterLevel = 381;
        illusionTemple5.MaximumSpecialCharacterLevel = 399;

        var illusionTemple6 = this.CreateIllusionTempleDefinition(6, IllusionTemple6.Number, 8000000);
        illusionTemple6.RequiresMasterClass = true;
        illusionTemple6.MinimumCharacterLevel = 0;
        illusionTemple6.MaximumCharacterLevel = 400;
        illusionTemple6.MinimumSpecialCharacterLevel = 0;
        illusionTemple6.MaximumSpecialCharacterLevel = 400;

        this.CreateSpecialSkillEffects();
    }

    /// <summary>
    /// Creates the magic effects used by the event's special skills (210 - Order of Protection and
    /// 211 - Restraint). The other two special skills (212 - Tracking and 213 - Weaken) act instantly
    /// and don't need a magic effect of their own.
    /// </summary>
    private void CreateSpecialSkillEffects()
    {
        var protection = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(protection);
        protection.Number = (short)MagicEffectNumber.IllusionTempleProtection;
        protection.Name = "Illusion Temple - Order of Protection";
        protection.InformObservers = true;
        protection.StopByDeath = true;
        protection.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        protection.Duration.ConstantValue!.Value = 15; // 15 seconds

        var protectionPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        protection.PowerUpDefinitions.Add(protectionPowerUp);
        protectionPowerUp.TargetAttribute = Stats.DamageReceiveDecrement.GetPersistent(this.GameConfiguration);
        protectionPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        protectionPowerUp.Boost.ConstantValue.Value = 0.50f; // 50 % damage reduction
        protectionPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var restraint = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(restraint);
        restraint.Number = (short)MagicEffectNumber.IllusionTempleRestraint;
        restraint.Name = "Illusion Temple - Restraint";
        restraint.InformObservers = true;
        restraint.StopByDeath = true;
        restraint.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        restraint.Duration.ConstantValue!.Value = 15; // 15 seconds

        var restraintPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        restraint.PowerUpDefinitions.Add(restraintPowerUp);
        restraintPowerUp.TargetAttribute = Stats.IsFrozen.GetPersistent(this.GameConfiguration);
        restraintPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        restraintPowerUp.Boost.ConstantValue.Value = 1;
    }

    /// <summary>
    /// Creates a new <see cref="MiniGameDefinition" /> for a illusion temple event.
    /// </summary>
    /// <param name="level">The level of the event.</param>
    /// <param name="mapNumber">The map number.</param>
    /// <param name="entranceFee">The entrance fee.</param>
    /// <returns>
    /// The created <see cref="MiniGameDefinition" />.
    /// </returns>
    protected MiniGameDefinition CreateIllusionTempleDefinition(byte level, short mapNumber, int entranceFee)
    {
        var illusionTemple = this.Context.CreateNew<MiniGameDefinition>();
        illusionTemple.SetGuid((short)MiniGameType.IllusionTemple, level);
        this.GameConfiguration.MiniGameDefinitions.Add(illusionTemple);
        illusionTemple.Name = $"Illusion Temple {level}";
        illusionTemple.Description = $"Event definition for illusion temple, level {level}.";
        illusionTemple.EnterDuration = TimeSpan.FromMinutes(5);
        illusionTemple.GameDuration = TimeSpan.FromMinutes(15);
        illusionTemple.ExitDuration = TimeSpan.FromMinutes(1);
        illusionTemple.MaximumPlayerCount = 10; // reduce it for small servers 4
        illusionTemple.MinimumPlayerCount = 2;
        illusionTemple.Entrance = this.GameConfiguration.Maps
            .First(m => m.Number == mapNumber)
            .ExitGates
            .Where(g => g.IsSpawnGate)
            .OrderBy(g => g.X1).ThenBy(g => g.Y1)
            .First();
        illusionTemple.Type = MiniGameType.IllusionTemple;
        illusionTemple.TicketItem = this.GameConfiguration.Items.Single(item => item is { Group: 13, Number: 51 });
        illusionTemple.TicketItemLevel = level;
        illusionTemple.GameLevel = level;
        illusionTemple.MapCreationPolicy = MiniGameMapCreationPolicy.Shared;
        illusionTemple.SaveRankingStatistics = true;
        illusionTemple.EntranceFee = entranceFee;
        illusionTemple.AllowParty = false;

        this.CreateRewards(level, illusionTemple);

        return illusionTemple;
    }

    private void CreateRewards(byte level, MiniGameDefinition illusionTemple)
    {
        var winnerExpReward = this.Context.CreateNew<MiniGameReward>();
        winnerExpReward.RewardType = MiniGameRewardType.Experience;
        winnerExpReward.RewardAmount = 100_000 * level;
        winnerExpReward.RequiredSuccess = MiniGameSuccessFlags.WinnerOrInWinningParty;
        illusionTemple.Rewards.Add(winnerExpReward);
    }
}