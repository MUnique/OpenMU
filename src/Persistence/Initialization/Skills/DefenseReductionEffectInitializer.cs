// <copyright file="DefenseReductionEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the defense reduction effect.
/// </summary>
public class DefenseReductionEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefenseReductionEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DefenseReductionEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.DefenseReduction;
        magicEffect.Name = "Defense Reduction Effect (Fire Slash)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = true;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = (float)TimeSpan.FromSeconds(10).TotalSeconds;

        var reducePvmDefenseEffect = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(reducePvmDefenseEffect);
        reducePvmDefenseEffect.TargetAttribute = Stats.DefensePvm.GetPersistent(this.GameConfiguration);
        reducePvmDefenseEffect.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        reducePvmDefenseEffect.Boost.ConstantValue.Value = 0.9f;
        reducePvmDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var reducePvpDefenseEffect = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(reducePvpDefenseEffect);
        reducePvpDefenseEffect.TargetAttribute = Stats.DefensePvp.GetPersistent(this.GameConfiguration);
        reducePvpDefenseEffect.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        reducePvpDefenseEffect.Boost.ConstantValue.Value = 0.9f;
        reducePvpDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}