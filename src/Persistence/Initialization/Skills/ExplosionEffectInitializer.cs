// <copyright file="ExplosionEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer for the explosion effect (book of samut).
/// </summary>
public class ExplosionEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ExplosionEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public ExplosionEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Explosion;
        magicEffect.Name = "Explosion Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();

        // Damage is applied 6 times for every second
        magicEffect.Duration.ConstantValue.Value = 6;

        var isBleedingPowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(isBleedingPowerUpDefinition);
        isBleedingPowerUpDefinition.TargetAttribute = Stats.IsBleeding.GetPersistent(this.GameConfiguration);
        isBleedingPowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        isBleedingPowerUpDefinition.Boost.ConstantValue.Value = 1;
    }
}