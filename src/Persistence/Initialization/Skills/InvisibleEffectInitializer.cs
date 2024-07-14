// <copyright file="InvisibleEffectInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Skills;

using System;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Initializer which initializes the invisible effect.
/// </summary>
public class InvisibleEffectInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvisibleEffectInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public InvisibleEffectInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    public override void Initialize()
    {
        var magicEffect = this.Context.CreateNew<MagicEffectDefinition>();
        this.GameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.Transparency;
        magicEffect.Name = "Invisible";
        magicEffect.InformObservers = false;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = false;
        magicEffect.Duration = this.Context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = (float)TimeSpan.FromDays(1).TotalSeconds;

        if (this.GameConfiguration.Attributes.All(a => Stats.IsInvisible.Id != a.Id))
        {
            var invisibleStat = this.Context.CreateNew<AttributeDefinition>(Stats.IsInvisible.Id, Stats.IsInvisible.Designation, Stats.IsInvisible.Description);
            this.GameConfiguration.Attributes.Add(invisibleStat);
        }

        var invisibleEffect = this.Context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(invisibleEffect);
        invisibleEffect.TargetAttribute = Stats.IsInvisible.GetPersistent(this.GameConfiguration);
        invisibleEffect.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        invisibleEffect.Boost.ConstantValue.Value = 1;
        invisibleEffect.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
    }
}