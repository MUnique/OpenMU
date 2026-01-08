// <copyright file="DecreaseBlockEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the decrease block effect which results from Phoenix Shot (Rage Fighter) skill.
/// </summary>
public class DecreaseBlockEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecreaseBlockEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DecreaseBlockEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.DecreaseBlock;
        magicEffect.Name = "Decrease Block Effect (Phoenix Shot)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 10; // 10 Seconds
        magicEffect.Chance = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.1f; // 10%

        var decDefRatePowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDefRatePowerUpDefinition);
        decDefRatePowerUpDefinition.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);
        decDefRatePowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDefRatePowerUpDefinition.Boost.ConstantValue.Value = -50f;
        decDefRatePowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;

        var decDefRatePowerUpDefinitionPvp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDefRatePowerUpDefinitionPvp);
        decDefRatePowerUpDefinitionPvp.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);
        decDefRatePowerUpDefinitionPvp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.Value = -20f;
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
    }
}