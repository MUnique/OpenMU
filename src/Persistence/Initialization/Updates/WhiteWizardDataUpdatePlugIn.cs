// <copyright file="WhiteWizardDataUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Adds White Wizard monster (id 135) and default drop groups for the White Wizard invasion.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("B7E1C3D5-2C36-489C-8E37-0C8D4D42D5A8")]
public class WhiteWizardDataUpdatePlugIn : UpdatePlugInBase
{
    internal const string PlugInName = "White Wizard Monster and Invasion Drops";
    internal const string PlugInDescription = "Adds White Wizard (135) and default drop groups for boss and support mobs (Season 6).";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.WhiteWizardAndInvasionDropsSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 03, 01, 0, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
#pragma warning disable CS1998
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
#pragma warning restore CS1998
    {
        CreateWhiteWizardIfMissing(context, gameConfiguration);
        CreateDefaultDropGroupsIfMissing(context, gameConfiguration);
    }

    private static void CreateWhiteWizardIfMissing(IContext context, GameConfiguration gameConfiguration)
    {
        if (gameConfiguration.Monsters.Any(m => m.Number == 135))
        {
            return;
        }

        var monster = context.CreateNew<MonsterDefinition>();
        gameConfiguration.Monsters.Add(monster);
        monster.Number = 135;
        monster.Designation = "White Wizard";
        monster.MoveRange = 3;
        monster.AttackRange = 4;
        monster.AttackSkill = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
        monster.ViewRange = 7;
        monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
        monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
        monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
        monster.Attribute = 2;
        monster.NumberOfMaximumItemDrops = 1;

        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, 90 },
            { Stats.MaximumHealth, 40000 },
            { Stats.MinimumPhysBaseDmg, 350 },
            { Stats.MaximumPhysBaseDmg, 420 },
            { Stats.DefenseBase, 260 },
            { Stats.AttackRatePvm, 450 },
            { Stats.DefenseRatePvm, 160 },
            { Stats.PoisonResistance, 10f / 255 },
            { Stats.IceResistance, 10f / 255 },
            { Stats.WaterResistance, 10f / 255 },
            { Stats.FireResistance, 10f / 255 },
        };
        monster.AddAttributes(attributes, context, gameConfiguration);
        monster.SetGuid(monster.Number);
    }

    private static void CreateDefaultDropGroupsIfMissing(IContext context, GameConfiguration gameConfiguration)
    {
        // Support drop group
        const string supportDropDescription = "WW Support Drop";
        if (!gameConfiguration.DropItemGroups.Any(g => g.Description == supportDropDescription))
        {
            var supportGroup = context.CreateNew<DropItemGroup>();
            supportGroup.Description = supportDropDescription;
            supportGroup.Chance = 1.0;

            var bless = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 13);
            var soul = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 14);
            var chaos = gameConfiguration.Items.First(i => i.Group == 12 && i.Number == 15);
            supportGroup.PossibleItems.Add(bless);
            supportGroup.PossibleItems.Add(soul);
            supportGroup.PossibleItems.Add(chaos);
            gameConfiguration.DropItemGroups.Add(supportGroup);
        }

        // Boss drop group
        const string bossDropDescription = "White Wizard Boss Drop";
        if (!gameConfiguration.DropItemGroups.Any(g => g.Description == bossDropDescription))
        {
            var bossGroup = context.CreateNew<DropItemGroup>();
            bossGroup.Description = bossDropDescription;
            bossGroup.Chance = 1.0;

            var guardian = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 31);
            var creation = gameConfiguration.Items.First(i => i.Group == 14 && i.Number == 22);
            var chaos = gameConfiguration.Items.First(i => i.Group == 12 && i.Number == 15);
            bossGroup.PossibleItems.Add(guardian);
            bossGroup.PossibleItems.Add(creation);
            bossGroup.PossibleItems.Add(chaos);
            gameConfiguration.DropItemGroups.Add(bossGroup);
        }
    }
}

