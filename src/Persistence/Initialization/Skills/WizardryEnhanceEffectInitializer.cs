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
/// <remarks>
/// It includes all attributes which are enhanced by the wizardry skill and its master skills.
/// </remarks>
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
        magicEffect.Duration.ConstantValue.Value = 1200f; // 20 minutes

        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.MinimumWizBaseDmg.GetPersistent(this.GameConfiguration);

        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 1.2f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var powerUpDefinition2 = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition2);
        powerUpDefinition2.TargetAttribute = Stats.MaximumWizBaseDmg.GetPersistent(this.GameConfiguration);

        powerUpDefinition2.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition2.Boost.ConstantValue.Value = 1f;
        powerUpDefinition2.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var powerUpDefinition3 = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition3);
        powerUpDefinition3.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(this.GameConfiguration);

        powerUpDefinition3.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition3.Boost.ConstantValue.Value = 1f;
        powerUpDefinition3.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}