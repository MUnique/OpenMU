// <copyright file="WeaknessEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the weakness effect which results from Killing Blow (RF) skill.
/// </summary>
public class WeaknessEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeaknessEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public WeaknessEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Weakness;
        magicEffect.Name = "Weakness Effect (Killing Blow)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 10; // 10 seconds

        // Chance to apply the effect
        magicEffect.Chance = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.1f; // 10%

        // Target's physical damage decreases by 5%
        var decDmgPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDmgPowerUpDefinition);
        decDmgPowerUpDefinition.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(this.GameConfiguration);
        decDmgPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
        decDmgPowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
    }
}