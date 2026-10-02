// <copyright file="AddDragonSlasherPveMultiplierPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the <see cref="Stats.SkillFinalMultiplierPve"/> attribute and lets Dragon Slasher
/// deal three times its damage to monsters through it. Before, this was hard-coded in the game logic.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("7DF15560-0A0D-404E-A947-6349E0AB8811")]
public class AddDragonSlasherPveMultiplierPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Add the PvE multiplier of Dragon Slasher";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "Dragon Slasher deals three times its damage to monsters. This was hard-coded and is now configured as skill attribute, which the new game logic requires.";

    /// <summary>
    /// The multiplier of the skill multiplier, which Dragon Slasher applies against monsters.
    /// </summary>
    private const float PveMultiplier = 3.0f;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 10, 02, 18, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Attributes.All(a => a.Id != Stats.SkillFinalMultiplierPve.Id))
        {
            var attribute = context.CreateNew<AttributeDefinition>(Stats.SkillFinalMultiplierPve.Id, Stats.SkillFinalMultiplierPve.Designation, Stats.SkillFinalMultiplierPve.Description);
            gameConfiguration.Attributes.Add(attribute);
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DragonSlasher) is not { } dragonSlasher
            || dragonSlasher.AttributeRelationships.Any(r => r.TargetAttribute == Stats.SkillFinalMultiplierPve))
        {
            return ValueTask.CompletedTask;
        }

        dragonSlasher.AttributeRelationships.Add(
            CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, Stats.SkillFinalMultiplierPve, PveMultiplier, Stats.SkillMultiplier));
        return ValueTask.CompletedTask;
    }
}
