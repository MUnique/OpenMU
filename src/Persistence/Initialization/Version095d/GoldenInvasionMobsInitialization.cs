// <copyright file="GoldenInvasionMobsInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization of all NPCs, which are no monsters.
/// </summary>
internal partial class GoldenInvasionMobsInitialization : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GoldenInvasionMobsInitialization" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GoldenInvasionMobsInitialization(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Creates all golden mobs.
    /// </summary>
    public override void Initialize()
    {
        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 43;
            monster.Designation = "Golden Budge Dragon";
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
                { Stats.Level, 15 },
                { Stats.MaximumHealth, 2500 },
                { Stats.MinimumPhysBaseDmg, 120 },
                { Stats.MaximumPhysBaseDmg, 125 },
                { Stats.DefenseBase, 45 },
                { Stats.AttackRatePvm, 75 },
                { Stats.DefenseRatePvm, 30 },
                { Stats.PoisonResistance, 2f / 255 },
                { Stats.IceResistance, 2f / 255 },
                { Stats.WaterResistance, 2f / 255 },
                { Stats.FireResistance, 2f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            var itemDrop = this.Context.CreateNew<DropItemGroup>();

            itemDrop.Chance = 1;
            itemDrop.Description = "Box of Luck";
            itemDrop.Monster = monster;
            itemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 14 && item.Number == 11));
            monster.DropItemGroups.Add(itemDrop);
            this.GameConfiguration.DropItemGroups.Add(itemDrop);
        }

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
            monster.Number = 54;
            monster.Designation = "Golden Soldier";
            monster.MoveRange = 3;
            monster.AttackRange = 5;
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(1800 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 46 },
                { Stats.MaximumHealth, 5000 },
                { Stats.MinimumPhysBaseDmg, 140 },
                { Stats.MaximumPhysBaseDmg, 150 },
                { Stats.DefenseBase, 75 },
                { Stats.AttackRatePvm, 230 },
                { Stats.DefenseRatePvm, 64 },
                { Stats.PoisonResistance, 3f / 255 },
                { Stats.IceResistance, 3f / 255 },
                { Stats.WaterResistance, 3f / 255 },
                { Stats.FireResistance, 3f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        }

        {
            var monster = this.Context.CreateNew<MonsterDefinition>();
            this.GameConfiguration.Monsters.Add(monster);
            monster.Number = 53;
            monster.Designation = "Golden Titan";
            monster.MoveRange = 3;
            monster.AttackRange = 3;
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MonsterSkill);
            monster.ViewRange = 7;
            monster.MoveDelay = new TimeSpan(400 * TimeSpan.TicksPerMillisecond);
            monster.AttackDelay = new TimeSpan(2000 * TimeSpan.TicksPerMillisecond);
            monster.RespawnDelay = new TimeSpan(600 * TimeSpan.TicksPerSecond);
            monster.Attribute = 2;
            monster.NumberOfMaximumItemDrops = 1;
            var attributes = new Dictionary<AttributeDefinition, float>
            {
                { Stats.Level, 53 },
                { Stats.MaximumHealth, 6500 },
                { Stats.MinimumPhysBaseDmg, 170 },
                { Stats.MaximumPhysBaseDmg, 180 },
                { Stats.DefenseBase, 105 },
                { Stats.AttackRatePvm, 265 },
                { Stats.DefenseRatePvm, 77 },
                { Stats.PoisonResistance, 5f / 255 },
                { Stats.IceResistance, 5f / 255 },
                { Stats.WaterResistance, 5f / 255 },
                { Stats.FireResistance, 5f / 255 },
            };
            monster.AddAttributes(attributes, this.Context, this.GameConfiguration);

            this.AddBoxOfKundunToMonster(2, monster);
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

    private void AddBoxOfKundunToMonster(byte lvl, MonsterDefinition monster)
    {
        var itemDrop = this.Context.CreateNew<DropItemGroup>();

        itemDrop.Chance = 1;
        itemDrop.ItemLevel = (byte)(7 + lvl);
        itemDrop.Description = $"Box of Kundun +{lvl}";
        itemDrop.Monster = monster;
        itemDrop.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == 14 && item.Number == 11));
        monster.DropItemGroups.Add(itemDrop);
        this.GameConfiguration.DropItemGroups.Add(itemDrop);
    }
}
