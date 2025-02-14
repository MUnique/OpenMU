// <copyright file="AlcoholEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the alcohol effect.
/// </summary>
public class AlcoholEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlcoholEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public AlcoholEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.Alcohol;
        magicEffect.SubType = 54;
        magicEffect.Name = "Alcohol Effect";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = false;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 80f;

        var attackSpeedPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(attackSpeedPowerUp);
        attackSpeedPowerUp.TargetAttribute = Stats.AttackSpeedAny.GetPersistent(this.GameConfiguration);
        attackSpeedPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        attackSpeedPowerUp.Boost.ConstantValue.Value = 20f;
    }
}