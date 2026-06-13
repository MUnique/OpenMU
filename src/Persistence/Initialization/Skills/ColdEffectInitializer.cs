// <copyright file="ColdEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the cold effect which slows movement speed.
/// </summary>
public class ColdEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ColdEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ColdEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Cold;
        magicEffect.Name = "Cold";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = true;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = (float)TimeSpan.FromSeconds(10).TotalSeconds;

        var movementSpeedFactorPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(movementSpeedFactorPowerUp);
        movementSpeedFactorPowerUp.TargetAttribute = Stats.MovementSpeedFactor.GetPersistent(this.GameConfiguration);
        movementSpeedFactorPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        movementSpeedFactorPowerUp.Boost.ConstantValue.Value = MovementSpeedConstants.ColdMovementSpeedFactor;
        movementSpeedFactorPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
    }
}
