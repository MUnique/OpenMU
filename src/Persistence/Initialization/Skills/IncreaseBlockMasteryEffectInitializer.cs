// <copyright file="IncreaseBlockMasteryEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the increase block mastery effect.
/// </summary>
public class IncreaseBlockMasteryEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseBlockMasteryEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseBlockMasteryEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockMastery;
        magicEffect.Name = "Increase Block Mastery Skill Effect";

        this.CopyMagicEffectValues(magicEffect, (short)MagicEffectNumber.IncreaseBlockPowerUp);

        var defensePowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(defensePowerUp);
        defensePowerUp.TargetAttribute = Stats.DefenseFinal.GetPersistent(this.GameConfiguration);
        defensePowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        defensePowerUp.Boost.ConstantValue.Value = 0f;
        defensePowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
    }
}