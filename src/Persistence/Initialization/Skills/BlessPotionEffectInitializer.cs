// <copyright file="BlessPotionEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the bless potion effect.
/// </summary>
public class BlessPotionEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BlessPotionEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public BlessPotionEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.PotionOfBless;
        magicEffect.SubType = 255 - (byte)MagicEffectNumber.PotionOfBless;
        magicEffect.Name = "Potion of Bless Effect";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 120;

        // TODO: This should only apply to castle gates and statues!
        var powerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUp);
        powerUp.TargetAttribute = Stats.AttackDamageIncrease.GetPersistent(this.GameConfiguration);
        powerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUp.Boost.ConstantValue.Value = 1.2f;
        powerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}