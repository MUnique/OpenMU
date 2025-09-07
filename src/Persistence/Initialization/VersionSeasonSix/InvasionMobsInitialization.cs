// <copyright file="InvasionMobsInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization of all monsters.
/// </summary>
internal class InvasionMobsInitialization : Version095d.InvasionMobsInitialization
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvasionMobsInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public InvasionMobsInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();
        this.CreateWhiteWizard();
        this.CreateWhiteWizardDefaultDropGroups();
    }

    /// <inheritdoc />
    protected override void InitializeGoldenInvasionMobs()
    {
        base.InitializeGoldenInvasionMobs();

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 78;
            monster.Designation = "Golden Goblin";
            monster.MoveRange = 3;
            monster.AttackRange = 1;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 20 },
                { Stats.MaximumHealth, 3200 },
                { Stats.MinimumPhysBaseDmg, 125 },
                { Stats.MaximumPhysBaseDmg, 130 },
                { Stats.DefenseBase, 50 },
                { Stats.AttackRatePvm, 100 },
                { Stats.DefenseRatePvm, 50 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.WaterResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            this.AddBoxOfKundunToMonster(1, monster);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 79;
            monster.Designation = "Golden Dragon";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 80 },
                { Stats.MaximumHealth, 22000 },
                { Stats.MinimumPhysBaseDmg, 300 },
                { Stats.MaximumPhysBaseDmg, 350 },
                { Stats.DefenseBase, 230 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 90 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            this.AddBoxOfKundunToMonster(3, monster);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 81;
            monster.Designation = "Golden Vepar";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 61 },
                { Stats.MaximumHealth, 10000 },
                { Stats.MinimumPhysBaseDmg, 190 },
                { Stats.MaximumPhysBaseDmg, 200 },
                { Stats.DefenseBase, 110 },
                { Stats.AttackRatePvm, 310 },
                { Stats.DefenseRatePvm, 90 },
                { Stats.WaterResistance, 3f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 80;
            monster.Designation = "Golden Lizard King";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Lightning);
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1600 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 83 },
                { Stats.MaximumHealth, 25000 },
                { Stats.MinimumPhysBaseDmg, 310 },
                { Stats.MaximumPhysBaseDmg, 360 },
                { Stats.DefenseBase, 240 },
                { Stats.AttackRatePvm, 415 },
                { Stats.DefenseRatePvm, 150 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            this.AddBoxOfKundunToMonster(4, monster);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 83;
            monster.Designation = "Golden Wheel";
            monster.MoveRange = 3;
            monster.AttackRange = 4;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 77 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 320 },
                { Stats.MaximumPhysBaseDmg, 360 },
                { Stats.DefenseBase, 230 },
                { Stats.AttackRatePvm, 385 },
                { Stats.DefenseRatePvm, 160 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
            monster.SetGuid(monster.Number);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 82;
            monster.Designation = "Golden Tantallos";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1400 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 90 },
                { Stats.MaximumHealth, 32000 },
                { Stats.MinimumPhysBaseDmg, 450 },
                { Stats.MaximumPhysBaseDmg, 560 },
                { Stats.DefenseBase, 300 },
                { Stats.AttackRatePvm, 450 },
                { Stats.DefenseRatePvm, 185 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            this.AddBoxOfKundunToMonster(5, monster);
        }
    }

    private void InitializeRedDragonInvasionMobs()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 44;
            monster.Designation = "Red Dragon";
            monster.MoveRange = 3;
            monster.AttackRange = 2;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(100 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 47 },
                { Stats.MaximumHealth, 15000 },
                { Stats.MinimumPhysBaseDmg, 190 },
                { Stats.MaximumPhysBaseDmg, 210 },
                { Stats.DefenseBase, 120 },
                { Stats.AttackRatePvm, 400 },
                { Stats.DefenseRatePvm, 88 },
                { Stats.PoisonResistance, 9f / 255 },
                { Stats.IceResistance, 7f / 255 },
                { Stats.WaterResistance, 9f / 255 },
                { Stats.FireResistance, 9f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            var itemDrop = this.Context.CreateNew<DropItemGroup>();

            itemDrop.Chance = 1;
            itemDrop.Description = "Items from red dragon";
            itemDrop.Monster = monster;
            itemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 14 && item.Number == 13)); // Jewel of Bless
            itemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 14 && item.Number == 14)); // Jewel of Soul
            itemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 12 && item.Number == 15)); // Jewel of Chaos
            monster.DropItemGroups.Add(itemDrop);
            this.GameConfiguration.DropItemGroups.Add(itemDrop);
        }
    }

    private void CreateWhiteWizard()
    {
        // MonsterId 135 as used by the client (NpcName_Eng.txt)
        if (this.GameConfiguration.Monsters.Any(m => m.Number == 135))
        {
            return; // already created
        }

        var monster = this.Context.CreateNew<MonsterDefinition>();
        this.GameConfiguration.Monsters.Add(monster);
        monster.Number = 135;
        monster.Designation = "White Wizard";
        monster.MoveRange = 3;
        monster.AttackRange = 4;
        monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EnergyBall);
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
        monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        monster.SetGuid(monster.Number);
    }

    private void CreateWhiteWizardDefaultDropGroups()
    {
        // Create a default support drop group (for Orcs) if not existing yet
        const string supportDropDescription = "WW Support Drop";
        if (!this.GameConfiguration.DropItemGroups.Any(g => g.Description == supportDropDescription))
        {
            var supportGroup = this.Context.CreateNew<DropItemGroup>();
            supportGroup.Description = supportDropDescription;
            supportGroup.Chance = 1.0; // always drop from this group when attached

            // Typical jewels
            var bless = this.GameConfiguration.Items.First(i => i.Group == 14 && i.Number == 13);
            var soul = this.GameConfiguration.Items.First(i => i.Group == 14 && i.Number == 14);
            var chaos = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 15);
            supportGroup.PossibleItems.Add(bless);
            supportGroup.PossibleItems.Add(soul);
            supportGroup.PossibleItems.Add(chaos);
            this.GameConfiguration.DropItemGroups.Add(supportGroup);
        }

        // Create a default boss drop group if not existing yet
        const string bossDropDescription = "White Wizard Boss Drop";
        if (!this.GameConfiguration.DropItemGroups.Any(g => g.Description == bossDropDescription))
        {
            var bossGroup = this.Context.CreateNew<DropItemGroup>();
            bossGroup.Description = bossDropDescription;
            bossGroup.Chance = 1.0; // always drop when attached

            // Better rewards: Guardian, Creation, Chaos, random excellent
            var guardian = this.GameConfiguration.Items.First(i => i.Group == 14 && i.Number == 31); // Jewel of Guardian
            var creation = this.GameConfiguration.Items.First(i => i.Group == 14 && i.Number == 22); // Jewel of Creation
            var chaos = this.GameConfiguration.Items.First(i => i.Group == 12 && i.Number == 15);   // Jewel of Chaos
            bossGroup.PossibleItems.Add(guardian);
            bossGroup.PossibleItems.Add(creation);
            bossGroup.PossibleItems.Add(chaos);

            this.GameConfiguration.DropItemGroups.Add(bossGroup);
        }
    }
}
