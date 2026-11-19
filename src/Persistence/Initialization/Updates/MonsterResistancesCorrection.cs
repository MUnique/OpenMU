// <copyright file="MonsterResistancesCorrection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// The corrected elemental resistances of a monster, which <see cref="MonsterResistancesUpdatePlugInBase"/> applies to
/// existing databases. All values are whole numbers.
/// </summary>
/// <param name="Number">The number of the monster.</param>
/// <param name="Previous">The resistances of the former initialization data, converted to whole numbers.
/// The correction is only applied when the monster still has them.</param>
/// <param name="Corrected">The corrected resistances.</param>
internal sealed record MonsterResistancesCorrection(
    short Number,
    (int Ice, int Poison, int Lightning, int Fire, int Water, int Wind) Previous,
    (int Ice, int Poison, int Lightning, int Fire) Corrected)
{
    /// <summary>
    /// Applies the corrected resistances to the monster of the game configuration, if the configuration has it and its
    /// resistances weren't customized.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public void ApplyTo(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == this.Number) is not { } monster)
        {
            return;
        }

        var current = (
            GetValue(monster, Stats.IceResistance),
            GetValue(monster, Stats.PoisonResistance),
            GetValue(monster, Stats.LightningResistance),
            GetValue(monster, Stats.FireResistance),
            GetValue(monster, Stats.WaterResistance),
            GetValue(monster, Stats.WindResistance));
        if (current != this.Previous)
        {
            return;
        }

        SetValue(context, gameConfiguration, monster, Stats.IceResistance, this.Corrected.Ice);
        SetValue(context, gameConfiguration, monster, Stats.PoisonResistance, this.Corrected.Poison);
        SetValue(context, gameConfiguration, monster, Stats.LightningResistance, this.Corrected.Lightning);
        SetValue(context, gameConfiguration, monster, Stats.FireResistance, this.Corrected.Fire);
        SetValue(context, gameConfiguration, monster, Stats.WaterResistance, 0);
        SetValue(context, gameConfiguration, monster, Stats.WindResistance, 0);
    }

    private static int GetValue(MonsterDefinition monster, AttributeDefinition attributeDefinition)
    {
        return monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeDefinition.Id) is { } attribute
            ? (int)MathF.Round(attribute.Value)
            : 0;
    }

    private static void SetValue(IContext context, GameConfiguration gameConfiguration, MonsterDefinition monster, AttributeDefinition attributeDefinition, int value)
    {
        var attribute = monster.Attributes.FirstOrDefault(a => a.AttributeDefinition?.Id == attributeDefinition.Id);
        if (value == 0)
        {
            // Like the initialization data, which doesn't list resistances of 0.
            if (attribute is not null)
            {
                monster.Attributes.Remove(attribute);
            }
        }
        else if (attribute is not null)
        {
            attribute.Value = value;
        }
        else
        {
            monster.AddAttributes(new Dictionary<AttributeDefinition, float> { { attributeDefinition, value } }, context, gameConfiguration);
        }
    }
}
