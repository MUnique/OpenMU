// <copyright file="AddWhiteWizardInvasionMobsUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds the White Wizard invasion monsters (135-137) and their drop groups
/// to existing databases where <see cref="VersionSeasonSix.InvasionMobsInitialization"/>
/// has already run but before White Wizard monsters were defined.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D8F4E2C0-5A6B-4C3D-9E7F-1B2A4C6D8E0F")]
public class AddWhiteWizardInvasionMobsUpdatePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// Gets the plugin name.
    /// </summary>
    internal const string PlugInName = "Add White Wizard Invasion Monsters";

    /// <summary>
    /// Gets the plugin description.
    /// </summary>
    internal const string PlugInDescription = "Adds White Wizard (135), Destructive Ogre Soldier (136), and Destructive Ogre Archer (137) and drop groups for existing databases.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.AddWhiteWizardInvasionMobs;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 07, 05, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.Any(m => m.Number == 135))
        {
            return ValueTask.CompletedTask;
        }

        this.AddWhiteWizard(context, gameConfiguration);
        this.AddDestructiveOgreSoldier(context, gameConfiguration);
        this.AddDestructiveOgreArcher(context, gameConfiguration);

        return ValueTask.CompletedTask;
    }

    private void AddWhiteWizard(IContext context, GameConfiguration gameConfiguration)
    {
        var monster = context.CreateNew<MonsterDefinition>();
        gameConfiguration.Monsters.Add(monster);
        monster.Number = 135;
        monster.Designation = "White Wizard";
        monster.MoveRange = 4;
        monster.AttackRange = 5;
        monster.AttackSkill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
        monster.ViewRange = 6;
        monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
        monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
        monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
        monster.Attribute = 2;
        monster.NumberOfMaximumItemDrops = 1;
        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, 87 },
            { Stats.MaximumHealth, 26000 },
            { Stats.MinimumPhysBaseDmg, 370 },
            { Stats.MaximumPhysBaseDmg, 410 },
            { Stats.DefenseBase, 400 },
            { Stats.AttackRatePvm, 550 },
            { Stats.DefenseRatePvm, 200 },
            { Stats.IceResistance, 15f / 255 },
            { Stats.PoisonResistance, 15f / 255 },
            { Stats.LightningResistance, 20f / 255 },
            { Stats.FireResistance, 15f / 255 },
        };
        monster.AddAttributes(attributes, context, gameConfiguration);

        var itemDrop = context.CreateNew<DropItemGroup>();
        itemDrop.Chance = 1.0;
        itemDrop.Description = "Jewel of Bless from White Wizard";
        itemDrop.Monster = monster;
        itemDrop.PossibleItems.Add(gameConfiguration.Items.First(item => item.Number == ItemConstants.JewelOfBless.Number && item.Group == ItemConstants.JewelOfBless.Group));
        monster.DropItemGroups.Add(itemDrop);
        gameConfiguration.DropItemGroups.Add(itemDrop);
    }

    private void AddDestructiveOgreSoldier(IContext context, GameConfiguration gameConfiguration)
    {
        var monster = context.CreateNew<MonsterDefinition>();
        gameConfiguration.Monsters.Add(monster);
        monster.Number = 136;
        monster.Designation = "Destructive Ogre Soldier";
        monster.MoveRange = 3;
        monster.AttackRange = 1;
        monster.AttackSkill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
        monster.ViewRange = 3;
        monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
        monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
        monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
        monster.Attribute = 2;
        monster.NumberOfMaximumItemDrops = 1;
        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, 70 },
            { Stats.MaximumHealth, 9500 },
            { Stats.MinimumPhysBaseDmg, 210 },
            { Stats.MaximumPhysBaseDmg, 240 },
            { Stats.DefenseBase, 180 },
            { Stats.AttackRatePvm, 400 },
            { Stats.DefenseRatePvm, 125 },
            { Stats.IceResistance, 7f / 255 },
            { Stats.PoisonResistance, 7f / 255 },
            { Stats.LightningResistance, 7f / 255 },
            { Stats.FireResistance, 7f / 255 },
        };
        monster.AddAttributes(attributes, context, gameConfiguration);

        var itemDrop = context.CreateNew<DropItemGroup>();
        itemDrop.Chance = 0.8;
        itemDrop.Description = "Wizard's Ring from Destructive Ogre Soldier";
        itemDrop.Monster = monster;
        itemDrop.PossibleItems.Add(gameConfiguration.Items.First(item => item.Number == ItemConstants.WizardsRing.Number && item.Group == ItemConstants.WizardsRing.Group));
        monster.DropItemGroups.Add(itemDrop);
        gameConfiguration.DropItemGroups.Add(itemDrop);
    }

    private void AddDestructiveOgreArcher(IContext context, GameConfiguration gameConfiguration)
    {
        var monster = context.CreateNew<MonsterDefinition>();
        gameConfiguration.Monsters.Add(monster);
        monster.Number = 137;
        monster.Designation = "Destructive Ogre Archer";
        monster.MoveRange = 3;
        monster.AttackRange = 5;
        monster.AttackSkill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
        monster.ViewRange = 5;
        monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
        monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
        monster.RespawnDelay = new TimeSpan(10 * TimeSpan.TicksPerSecond);
        monster.Attribute = 2;
        monster.NumberOfMaximumItemDrops = 1;
        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, 74 },
            { Stats.MaximumHealth, 12000 },
            { Stats.MinimumPhysBaseDmg, 220 },
            { Stats.MaximumPhysBaseDmg, 260 },
            { Stats.DefenseBase, 190 },
            { Stats.AttackRatePvm, 440 },
            { Stats.DefenseRatePvm, 130 },
            { Stats.IceResistance, 8f / 255 },
            { Stats.PoisonResistance, 8f / 255 },
            { Stats.LightningResistance, 8f / 255 },
            { Stats.FireResistance, 8f / 255 },
        };
        monster.AddAttributes(attributes, context, gameConfiguration);

        var itemDrop = context.CreateNew<DropItemGroup>();
        itemDrop.Chance = 0.8;
        itemDrop.Description = "Wizard's Ring from Destructive Ogre Archer";
        itemDrop.Monster = monster;
        itemDrop.PossibleItems.Add(gameConfiguration.Items.First(item => item.Number == ItemConstants.WizardsRing.Number && item.Group == ItemConstants.WizardsRing.Group));
        monster.DropItemGroups.Add(itemDrop);
        gameConfiguration.DropItemGroups.Add(itemDrop);
    }
}
