// <copyright file="WizardryEnhanceEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the wizardry enhance effect.
/// </summary>
public class WizardryEnhanceEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WizardryEnhanceEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public WizardryEnhanceEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.WizEnhance;
        magicEffect.Name = "Wiz Enhance Skill Effect";
        magicEffect.InformObservers = true;
        magicEffect.SubType = 33;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 1800f; // 30 minutes

        var minWizDmgPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(minWizDmgPowerUp);
        minWizDmgPowerUp.TargetAttribute = Stats.MinimumWizBaseDmg.GetPersistent(this.GameConfiguration);
        minWizDmgPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        minWizDmgPowerUp.Boost.MaximumValue = 100f; // 100 dmg

        var minWizDmgPerEnergy = this.Context.CreateNew<AttributeRelationship>();
        minWizDmgPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(this.GameConfiguration);
        minWizDmgPerEnergy.InputOperator = InputOperator.Multiply;
        minWizDmgPerEnergy.InputOperand = 0.2f / 9; // 45 energy adds 1 min wiz damage
        minWizDmgPowerUp.Boost.RelatedValues.Add(minWizDmgPerEnergy);
    }
}