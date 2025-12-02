// <copyright file="GameConfigurationInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using System.Reflection;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Events;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

/// <summary>
/// Initializes the <see cref="GameConfiguration"/>.
/// </summary>
public class GameConfigurationInitializer : GameConfigurationInitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameConfigurationInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    protected override IEnumerable<ItemOptionType> OptionTypes
    {
        get
        {
            // we take all in season 6
            return typeof(ItemOptionTypes)
                .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(p => p.PropertyType == typeof(ItemOptionType))
                .Select(p => p.GetValue(typeof(ItemOptionType)))
                .OfType<ItemOptionType>()
                .ToList();
        }
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.BaseDamageBonus, ItemOptionDefinitionNumbers.PhysicalAndWizardryAttack));
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.CurseBaseDmg, ItemOptionDefinitionNumbers.CurseAttack));

        new CharacterClassInitialization(this.Context, this.GameConfiguration).Initialize();
        new SkillsInitializer(this.Context, this.GameConfiguration).Initialize();
        new Orbs(this.Context, this.GameConfiguration).Initialize();
        new Scrolls(this.Context, this.GameConfiguration).Initialize();
        new EventTicketItems(this.Context, this.GameConfiguration).Initialize();
        new Wings(this.Context, this.GameConfiguration).Initialize();
        new Pets(this.Context, this.GameConfiguration).Initialize();
        new ExcellentOptions(this.Context, this.GameConfiguration).Initialize();
        new HarmonyOptions(this.Context, this.GameConfiguration).Initialize();
        new GuardianOptions(this.Context, this.GameConfiguration).Initialize();
        new Armors(this.Context, this.GameConfiguration).Initialize();
        new Weapons(this.Context, this.GameConfiguration).Initialize();
        new Potions(this.Context, this.GameConfiguration).Initialize();
        new Jewels(this.Context, this.GameConfiguration).Initialize();
        new Misc(this.Context, this.GameConfiguration).Initialize();
        new PackedJewels(this.Context, this.GameConfiguration).Initialize();
        new Jewelery(this.Context, this.GameConfiguration).Initialize();
        new AncientSets(this.Context, this.GameConfiguration).Initialize();
        new BoxOfLuck(this.Context, this.GameConfiguration).Initialize();
        this.CreateJewelMixes();
        new NpcInitialization(this.Context, this.GameConfiguration).Initialize();
        new InvasionMobsInitialization(this.Context, this.GameConfiguration).Initialize();
        new GameMapsInitializer(this.Context, this.GameConfiguration).Initialize();
        this.AssignCharacterClassHomeMaps();
        new SocketSystem(this.Context, this.GameConfiguration).Initialize();
        new ChaosMixes(this.Context, this.GameConfiguration).Initialize();
        new Gates(this.Context, this.GameConfiguration).Initialize();
        new Quest(this.Context, this.GameConfiguration).Initialize();
        new Quests(this.Context, this.GameConfiguration).Initialize();
        new DevilSquareInitializer(this.Context, this.GameConfiguration).Initialize();
        new BloodCastleInitializer(this.Context, this.GameConfiguration).Initialize();
        new ChaosCastleInitializer(this.Context, this.GameConfiguration).Initialize();
    }

    private void CreateJewelMixes()
    {
        this.CreateJewelMix(0, 13, 0xE, 30); // Bless
        this.CreateJewelMix(1, 14, 0xE, 31); // Soul
        this.CreateJewelMix(2, 16, 0xE, 136); // Jewel of Life
        this.CreateJewelMix(3, 22, 0xE, 137); // Jewel of Creation
        this.CreateJewelMix(4, 31, 0xE, 138); // Jewel of Guardian
        this.CreateJewelMix(5, 41, 0xE, 139); // Gemstone
        this.CreateJewelMix(6, 42, 0xE, 140); // Jewel of Harmony
        this.CreateJewelMix(7, 15, 0xC, 141); // Chaos
        this.CreateJewelMix(8, 43, 0xE, 142); // Lower Refine Stone
        this.CreateJewelMix(9, 44, 0xE, 143); // Higher Refine Stone
    }

    private void CreateJewelMix(byte mixNumber, int itemNumber, int itemGroup, int packedJewelId)
    {
        var singleJewel = this.GameConfiguration.Items.First(i => i.Group == itemGroup && i.Number == itemNumber);
        var packedJewel = this.GameConfiguration.Items.First(i => i.Group == 0x0C && i.Number == packedJewelId);
        var jewelMix = this.Context.CreateNew<JewelMix>();
        jewelMix.SetGuid(mixNumber);
        jewelMix.Number = mixNumber;
        jewelMix.SingleJewel = singleJewel;
        jewelMix.MixedJewel = packedJewel;
        this.GameConfiguration.JewelMixes.Add(jewelMix);
    }
}