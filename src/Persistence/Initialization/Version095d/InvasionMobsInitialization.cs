// <copyright file="InvasionMobsInitialization.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initialization of all monsters.
/// </summary>
internal class InvasionMobsInitialization : InitializerBase
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

    /// <summary>
    /// Creates all mobs.
    /// </summary>
    public override void Initialize()
    {
        this.InitializeGoldenInvasionMobs();
        this.InitializeRedDragonInvasionMobs();
    }

    /// <summary>
    /// Initializes the golden invasion mobs.
    /// </summary>
    protected virtual void InitializeGoldenInvasionMobs()
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
            monster.SetGuid(monster.Number);
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
    }

    /// <summary>
    /// Adds the box of kundun to monster.
    /// </summary>
    /// <param name="lvl">The level of the box.</param>
    /// <param name="monster">The monster.</param>
    protected void AddBoxOfKundunToMonster(byte lvl, MonsterDefinition monster)
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
}
