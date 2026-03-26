// <copyright file="StunEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the stun effect.
/// </summary>
public class StunEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StunEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public StunEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Stunned;
        magicEffect.Name = "Stun Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;

        var isStunnedPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(isStunnedPowerUpDefinition);
        isStunnedPowerUpDefinition.TargetAttribute = Stats.IsStunned.GetPersistent(this.GameConfiguration);
        isStunnedPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        isStunnedPowerUpDefinition.Boost.ConstantValue.Value = 1;
    }
}