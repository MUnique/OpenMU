// <copyright file="SoulPotionEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the soul potion effect.
/// </summary>
public class SoulPotionEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoulPotionEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public SoulPotionEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.PotionOfSoul;
        magicEffect.SubType = 255 - (byte)MagicEffectNumber.PotionOfSoul;
        magicEffect.Name = "Potion of Soul Effect";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 60;

        var attackSpeedPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(attackSpeedPowerUp);
        attackSpeedPowerUp.TargetAttribute = Stats.AttackSpeedAny.GetPersistent(this.GameConfiguration);
        attackSpeedPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        attackSpeedPowerUp.Boost.ConstantValue.Value = 20f;

        var abilityRecoveryIncrease = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(abilityRecoveryIncrease);
        abilityRecoveryIncrease.TargetAttribute = Stats.AbilityRecoveryAbsolute.GetPersistent(this.GameConfiguration);
        abilityRecoveryIncrease.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        abilityRecoveryIncrease.Boost.ConstantValue.Value = 8f;

        var lightningResistance = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(lightningResistance);
        lightningResistance.TargetAttribute = Stats.LightningResistance.GetPersistent(this.GameConfiguration);
        lightningResistance.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        lightningResistance.Boost.ConstantValue.Value = 0.5f;

        var iceResistance = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(iceResistance);
        iceResistance.TargetAttribute = Stats.IceResistance.GetPersistent(this.GameConfiguration);
        iceResistance.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        iceResistance.Boost.ConstantValue.Value = 0.5f;
    }
}