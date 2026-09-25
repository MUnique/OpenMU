// <copyright file="DoppelgangerMonsters.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// The initializer for the monsters of the doppelganger event.
/// </summary>
/// <remarks>
/// The values are default values, which can be adjusted in the admin panel.
/// </remarks>
internal class DoppelgangerMonsters : InitializerBase
{
    /// <summary>
    /// The number of the first monster of the doppelganger event.
    /// </summary>
    internal const short FirstMonsterNumber = 529;

    /// <summary>
    /// The number of the interim reward chest.
    /// </summary>
    internal const short InterimRewardChestNumber = 541;

    /// <summary>
    /// The number of the final reward chest.
    /// </summary>
    internal const short FinalRewardChestNumber = 542;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerMonsters" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public DoppelgangerMonsters(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateMonster(529, "Terrible Butcher", 20, 2800, 125, 135, 83, 2000, 1500, 3, 6, 7, 500, 1600, 0, null, 15, 254);
        this.CreateMonster(530, "Mad Butcher", 18, 2300, 110, 120, 73, 1000, 1000, 3, 6, 7, 500, 1600, 0, null, 15, 254);
        this.CreateMonster(531, "Ice Walker", 19, 1500, 85, 90, 60, 800, 1000, 3, 8, 10, 400, 2000, 2, null, 15, 15);
        this.CreateMonster(532, "Larva", 16, 1000, 75, 80, 55, 800, 900, 3, 2, 7, 400, 2000, 2, null, 15, 15);
        this.CreateMonster(533, "Doppelganger", 15, 600, 80, 85, 45, 1000, 900, 3, 1, 3, 400, 1600, 2, SkillNumber.DoppelgangerSelfExplosion, 10, 10);
        this.CreateMonster(534, "Doppelganger Elf", 14, 500, 65, 70, 30, 700, 500, 2, 5, 6, 400, 1400, 2, SkillNumber.MultiShot, 10, 10);
        this.CreateMonster(535, "Doppelganger Knight", 14, 500, 65, 70, 30, 700, 500, 2, 1, 3, 400, 2200, 2, SkillNumber.TwistingSlash, 10, 10);
        this.CreateMonster(536, "Doppelganger Wizard", 14, 500, 65, 70, 30, 700, 500, 2, 5, 4, 400, 2200, 2, SkillNumber.IceStorm, 10, 10);
        this.CreateMonster(537, "Doppelganger Magic Gladiator", 14, 500, 65, 70, 30, 700, 500, 2, 4, 4, 400, 1400, 2, SkillNumber.PowerSlash, 10, 10);
        this.CreateMonster(538, "Doppelganger Dark Lord", 17, 900, 85, 90, 55, 900, 800, 3, 4, 6, 400, 1600, 2, SkillNumber.Earthshake, 15, 15);
        this.CreateMonster(539, "Doppelganger Summoner", 14, 500, 65, 70, 30, 500, 300, 3, 2, 4, 400, 1600, 2, SkillNumber.LightningShock, 10, 10);

        this.ConfigureRewardChests();
    }

    /// <summary>
    /// Configures the reward chests, which were created as passive NPCs before, so that they can
    /// be opened by attacking them. They drop jewels and with a small chance a Loch's Feather
    /// or Crest of Monarch.
    /// </summary>
    internal void ConfigureRewardChests()
    {
        this.ConfigureRewardChest(InterimRewardChestNumber, 2, true);
        this.ConfigureRewardChest(FinalRewardChestNumber, 5, false);
    }

    private void ConfigureRewardChest(short number, int numberOfDrops, bool isJewelGuaranteed)
    {
        if (this.GameConfiguration.Monsters.FirstOrDefault(monster => monster.Number == number) is not { } chest
            || chest.ObjectKind == NpcObjectKind.Destructible)
        {
            return;
        }

        chest.ObjectKind = NpcObjectKind.Destructible;
        chest.NumberOfMaximumItemDrops = numberOfDrops;
        chest.RespawnDelay = TimeSpan.Zero;
        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, 2 },
            { Stats.MaximumHealth, 10 },
            { Stats.DefenseBase, 10 },
            { Stats.DefenseRatePvm, 10 },
        };
        chest.AddAttributes(attributes, this.Context, this.GameConfiguration);

        var jewels = new[] { (12, 15), (14, 13), (14, 14), (14, 16), (14, 22) };
        this.AddDropGroup(chest, 1, "Jewel", isJewelGuaranteed ? 1.0 : 0.9, 0, jewels);
        this.AddDropGroup(chest, 2, "Loch's Feather", 0.08, 0, (13, 14));
        this.AddDropGroup(chest, 3, "Crest of Monarch", 0.02, 1, (13, 14));
    }

    private void AddDropGroup(MonsterDefinition chest, short index, string name, double chance, byte itemLevel, params (int Group, int Number)[] items)
    {
        var group = this.Context.CreateNew<DropItemGroup>();
        group.SetGuid(chest.Number, index);
        group.Description = $"{chest.Designation}: {name}";
        group.Chance = chance;
        group.ItemLevel = itemLevel;
        group.Monster = chest;
        foreach (var (itemGroup, itemNumber) in items)
        {
            group.PossibleItems.Add(this.GameConfiguration.Items.First(item => item.Group == itemGroup && item.Number == itemNumber));
        }

        chest.DropItemGroups.Add(group);
        this.GameConfiguration.DropItemGroups.Add(group);
    }

    private void CreateMonster(
        short number,
        string designation,
        byte level,
        int health,
        int minimumDamage,
        int maximumDamage,
        int defense,
        int attackRate,
        int defenseRate,
        byte moveRange,
        byte attackRange,
        short viewRange,
        int moveDelayMs,
        int attackDelayMs,
        byte attribute,
        SkillNumber? attackSkill,
        byte resistance,
        byte lightningResistance)
    {
        var monster = this.Context.CreateNew<MonsterDefinition>();
        this.GameConfiguration.Monsters.Add(monster);
        monster.Number = number;
        monster.Designation = designation;
        monster.MoveRange = moveRange;
        monster.AttackRange = attackRange;
        monster.ViewRange = viewRange;
        monster.MoveDelay = TimeSpan.FromMilliseconds(moveDelayMs);
        monster.AttackDelay = TimeSpan.FromMilliseconds(attackDelayMs);
        monster.RespawnDelay = TimeSpan.FromSeconds(10);
        monster.Attribute = attribute;
        monster.NumberOfMaximumItemDrops = 1;
        if (attackSkill is { } skillNumber)
        {
            monster.AttackSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber);
        }

        var attributes = new Dictionary<AttributeDefinition, float>
        {
            { Stats.Level, level },
            { Stats.MaximumHealth, health },
            { Stats.MinimumPhysBaseDmg, minimumDamage },
            { Stats.MaximumPhysBaseDmg, maximumDamage },
            { Stats.DefenseBase, defense },
            { Stats.AttackRatePvm, attackRate },
            { Stats.DefenseRatePvm, defenseRate },
            { Stats.PoisonResistance, resistance / 255f },
            { Stats.IceResistance, resistance / 255f },
            { Stats.LightningResistance, lightningResistance / 255f },
            { Stats.FireResistance, resistance / 255f },
        };

        monster.AddAttributes(attributes, this.Context, this.GameConfiguration);
        monster.SetGuid(monster.Number);
    }
}
