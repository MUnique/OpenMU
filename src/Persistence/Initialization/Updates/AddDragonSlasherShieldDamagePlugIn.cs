// <copyright file="AddDragonSlasherShieldDamagePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the chance effect of Dragon Slasher to decrease the defender's shield by a certain rate.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("F364DE3C-D551-4839-A171-EB66F314D755")]
public class AddDragonSlasherShieldDamagePlugIn : UpdatePlugInBase
{
    private const string PlugInName = "Add Dragon Slasher Shield Damage";

    private const string PlugInDescription = "Adds the chance effect of Dragon Slasher to decrease the defender's shield by a certain rate.";

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 10, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        this.AddStatIfNotExists(context, gameConfiguration, Stats.DragonSlasherShieldDamageChance);
        this.AddStatIfNotExists(context, gameConfiguration, Stats.DragonSlasherShieldDamageRate);

        var dragonSlasherShieldDamageChance = Stats.DragonSlasherShieldDamageChance.GetPersistent(gameConfiguration);
        var dragonSlasherShieldDamageRate = Stats.DragonSlasherShieldDamageRate.GetPersistent(gameConfiguration);

        if (gameConfiguration.Skills.FirstOrDefault(skill => skill.Number == (short)SkillNumber.DragonSlasher) is { } dragonSlasher)
        {
            this.AddAttributeRelationship(context, gameConfiguration, dragonSlasher, dragonSlasherShieldDamageChance, 0.1f, Stats.MaximumHealth, InputOperator.Minimum);
            this.AddAttributeRelationship(context, gameConfiguration, dragonSlasher, dragonSlasherShieldDamageChance, 1.0f / 10000, Stats.TotalEnergy);
            this.AddAttributeRelationship(context, gameConfiguration, dragonSlasher, dragonSlasherShieldDamageRate, 0.1f, Stats.MaximumHealth, InputOperator.Minimum);
            this.AddAttributeRelationship(context, gameConfiguration, dragonSlasher, dragonSlasherShieldDamageRate, 1.0f / 3000, Stats.TotalEnergy);
        }

        return ValueTask.CompletedTask;
    }

    private void AddAttributeRelationship(IContext context, GameConfiguration gameConfiguration, Skill dragonSlasher, AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
    {
        var relationship = CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, multiplier, sourceAttribute, inputOperator, aggregateType);
        dragonSlasher.AttributeRelationships.Add(relationship);
    }
}
