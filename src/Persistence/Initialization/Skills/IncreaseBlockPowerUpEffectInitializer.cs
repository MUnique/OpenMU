// <copyright file="IncreaseBlockPowerUpEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the increase block power up effect.
/// </summary>
public class IncreaseBlockPowerUpEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncreaseBlockPowerUpEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public IncreaseBlockPowerUpEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockPowerUp;
        magicEffect.Name = "Increase Block Power Up Skill Effect";

        this.CopyMagicEffectValues(magicEffect, (short)MagicEffectNumber.IncreaseBlock);

        var increaseBlockPowerUp = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(increaseBlockPowerUp);
        increaseBlockPowerUp.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(this.GameConfiguration);
        increaseBlockPowerUp.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseBlockPowerUp.Boost.ConstantValue.Value = 0f;
        increaseBlockPowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
    }
}