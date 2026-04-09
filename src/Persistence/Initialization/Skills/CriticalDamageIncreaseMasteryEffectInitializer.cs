// <copyright file="CriticalDamageIncreaseMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the critical damage increase mastery effect.
/// </summary>
public class CriticalDamageIncreaseMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CriticalDamageIncreaseMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public CriticalDamageIncreaseMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.CriticalDamageIncreaseMastery;
        magicEffect.Name = "Critical Damage Increase Mastery Skill Effect";

        var effect = this.GameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.CriticalDamageIncrease);
        magicEffect.InformObservers = effect.InformObservers;
        magicEffect.SendDuration = effect.SendDuration;
        magicEffect.StopByDeath = effect.StopByDeath;
        magicEffect.Duration = effect.Duration;
        foreach (var powerUpDefinition in effect.PowerUpDefinitions)
        {
            magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        }

        var critChancePowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUpDefinition);
        critChancePowerUpDefinition.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(this.GameConfiguration);
        critChancePowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        critChancePowerUpDefinition.Boost.ConstantValue.Value = 0;
    }
}