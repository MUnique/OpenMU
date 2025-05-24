// <copyright file="DefenseEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the defense effect of a shield.
/// </summary>
public class DefenseEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefenseEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DefenseEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.ShieldSkill;
        magicEffect.Name = "Shield Defense Skill Effect";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue!.Value = 4; // 4 Seconds

        var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.DamageReceiveDecrement.GetPersistent(this.GameConfiguration);

        // Always a 50 % damage reduction
        powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 0.50f;
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}