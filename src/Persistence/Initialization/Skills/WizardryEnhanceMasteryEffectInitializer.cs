// <copyright file="WizardryEnhanceMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the wizardry enhance mastery effect.
/// </summary>
public class WizardryEnhanceMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WizardryEnhanceMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public WizardryEnhanceMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.WizEnhanceMastery;
        magicEffect.Name = "Wizardry Enhance Mastery Skill Effect";

        this.CopyMagicEffectValues(magicEffect, (short)MagicEffectNumber.WizEnhanceStrengthener);

        var critChancePowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUp);
        critChancePowerUp.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(this.GameConfiguration);
        critChancePowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
    }
}