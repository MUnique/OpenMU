// <copyright file="LuckyItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Craftings;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Initializer for the lucky items: the lucky armor sets, the lucky item tickets and the jewels of extension and elevation.
/// </summary>
/// <remarks>
/// The data is taken from the item data of the open source client (sven-n/MuMain, src/bin/Data/Items), where
/// these items are tagged as lucky item or lucky item ticket.
/// The lucky armors share their names with the regular armors, but they are separate items. They can't be upgraded
/// with jewels of bless, soul or life, so their maximum item level is 0 and their only possible option is the
/// harmony option.
/// </remarks>
internal class LuckyItems : InitializerBase
{
    /// <summary>
    /// The number of the crafting of the lucky item exchange, which the client sends when the player exchanges a ticket.
    /// </summary>
    internal const byte TicketExchangeCraftingNumber = 51;

    /// <summary>
    /// The level requirement of the lucky armors of the first tier.
    /// </summary>
    private const int FirstTierLevelRequirement = 1;

    /// <summary>
    /// The level requirement of the lucky armors of the second tier.
    /// </summary>
    private const int SecondTierLevelRequirement = 150;

    /// <summary>
    /// The lucky item tickets (group 13), with the item group of the armors they're exchanged for, and the level requirement of their tier.
    /// </summary>
    private static readonly (short TicketNumber, byte ArmorGroup, int LevelRequirement)[] Tickets =
    [
        (135, 8, FirstTierLevelRequirement), // 1st Lucky Armor Ticket
        (136, 9, FirstTierLevelRequirement), // 1st Lucky Pants Ticket
        (137, 7, FirstTierLevelRequirement), // 1st Lucky Helm Ticket
        (138, 10, FirstTierLevelRequirement), // 1st Lucky Gloves Ticket
        (139, 11, FirstTierLevelRequirement), // 1st Lucky Boots Ticket
        (140, 8, SecondTierLevelRequirement), // 2nd Lucky Armor Ticket
        (141, 9, SecondTierLevelRequirement), // 2nd Lucky Pants Ticket
        (142, 7, SecondTierLevelRequirement), // 2nd Lucky Helm Ticket
        (143, 10, SecondTierLevelRequirement), // 2nd Lucky Gloves Ticket
        (144, 11, SecondTierLevelRequirement), // 2nd Lucky Boots Ticket
    ];

    private readonly List<ItemDefinition> _createdItems = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LuckyItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public LuckyItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the item numbers of the lucky armors, which are the same for helms, armors, pants, gloves and boots.
    /// </summary>
    internal static short FirstLuckyArmorNumber => 62;

    /// <summary>
    /// Gets the item number of the last lucky armor.
    /// </summary>
    internal static short LastLuckyArmorNumber => 72;

    /// <summary>
    /// Gets the item numbers (of group 13) of the lucky item tickets.
    /// </summary>
    internal static IEnumerable<short> TicketNumbers => Tickets.Select(t => t.TicketNumber);

    /// <inheritdoc />
    public override void Initialize()
    {
        this.CreateItems();
    }

    /// <summary>
    /// Determines whether the item definition is a lucky armor.
    /// </summary>
    /// <param name="item">The item definition.</param>
    /// <returns><c>true</c>, if the item definition is a lucky armor.</returns>
    internal static bool IsLuckyArmor(ItemDefinition item) => item.Group is >= 7 and <= 11 && item.Number >= FirstLuckyArmorNumber && item.Number <= LastLuckyArmorNumber;

    /// <summary>
    /// Determines whether the item definition is one of the lucky items of this initializer.
    /// </summary>
    /// <param name="item">The item definition.</param>
    /// <returns><c>true</c>, if the item definition is one of the lucky items.</returns>
    internal static bool IsLuckyItem(ItemDefinition item) =>
        IsLuckyArmor(item)
        || (item.Group == 13 && TicketNumbers.Contains(item.Number))
        || (item.Group == 14 && item.Number is 160 or 161);

    /// <summary>
    /// Creates the lucky items which don't exist yet.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateItems()
    {
        this._createdItems.Clear();
        this.CreateArmors();
        this.CreateTickets();
        this.CreateJewels();
        return this._createdItems.ToList();
    }

    /// <summary>
    /// Creates the crafting of the lucky item exchange, which exchanges a lucky item ticket for the
    /// lucky armor of its tier and item group, which fits to the character class of the player.
    /// </summary>
    /// <returns>The crafting.</returns>
    internal ItemCrafting CreateTicketExchangeCrafting()
    {
        var crafting = this.Context.CreateNew<ItemCrafting>();
        crafting.Name = LocalizedString.FromResource(() => CraftingNames.LuckyItemExchange);
        crafting.Number = TicketExchangeCraftingNumber;
        crafting.ItemCraftingHandlerClassName = typeof(LuckyItemTicketExchangeCrafting).FullName!;
        var craftingSettings = this.Context.CreateNew<SimpleCraftingSettings>();
        crafting.SimpleCraftingSettings = craftingSettings;
        craftingSettings.SuccessPercent = 100;

        byte reference = 0;
        foreach (var (ticketNumber, armorGroup, levelRequirement) in Tickets)
        {
            reference++;
            var ticket = this.GameConfiguration.Items.First(item => item.Group == 13 && item.Number == ticketNumber);
            var requiredTicket = this.Context.CreateNew<ItemCraftingRequiredItem>();
            requiredTicket.PossibleItems.Add(ticket);
            requiredTicket.MinimumAmount = 0;
            requiredTicket.MaximumAmount = 1;
            requiredTicket.Reference = reference;
            craftingSettings.RequiredItems.Add(requiredTicket);

            var armors = this.GameConfiguration.Items
                .Where(item => item.Group == armorGroup && IsLuckyArmor(item))
                .Where(item => item.Requirements.Any(r => r.Attribute == Stats.Level && r.MinimumValue == levelRequirement))
                .OrderBy(item => item.Number);
            foreach (var armor in armors)
            {
                var result = this.Context.CreateNew<ItemCraftingResultItem>();
                result.ItemDefinition = armor;
                result.Reference = reference;
                craftingSettings.ResultItems.Add(result);
            }
        }

        return crafting;
    }

    private void CreateArmors()
    {
        // Helms:
        this.CreateLuckyArmor(62, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleHelm), 26, 16, 1, CharacterClasses.AllKnights | CharacterClasses.AllLords | CharacterClasses.AllFighters);
        this.CreateLuckyArmor(63, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkHelm), 16, 11, 1, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(64, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxHelm), 32, 17, 1, CharacterClasses.AllWizards);
        this.CreateLuckyArmor(65, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.ViolentWindHelm), 28, 17, 1, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(66, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantineMask), 66, 29, 150, CharacterClasses.AllLords);
        this.CreateLuckyArmor(67, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonHelm), 57, 29, 150, CharacterClasses.AllKnights);
        this.CreateLuckyArmor(68, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianHelm), 53, 28, 150, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(69, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryHelm), 50, 23, 150, CharacterClasses.AllWizards);
        this.CreateLuckyArmor(70, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingHelm), 50, 20, 150, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(72, 7, 2, 2, LocalizedString.FromResource(() => ItemNames.StormJahadHelm), 70, 32, 150, CharacterClasses.AllFighters);

        // Armors:
        this.CreateLuckyArmor(62, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleArmor), 28, 21, 1, CharacterClasses.AllKnights | CharacterClasses.AllMGs | CharacterClasses.AllLords | CharacterClasses.AllFighters);
        this.CreateLuckyArmor(63, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkArmor), 20, 14, 1, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(64, 8, 2, 3, LocalizedString.FromResource(() => ItemNames.SphinxArmor), 38, 19, 1, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(65, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.ViolentWindArmor), 34, 26, 1, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(66, 8, 2, 3, LocalizedString.FromResource(() => ItemNames.AdamantineArmor), 78, 39, 150, CharacterClasses.AllLords);
        this.CreateLuckyArmor(67, 8, 2, 3, LocalizedString.FromResource(() => ItemNames.DragonArmor), 59, 42, 150, CharacterClasses.AllKnights | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(68, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianArmor), 57, 29, 150, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(69, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryArmor), 56, 24, 150, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(70, 8, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingArmor), 56, 31, 150, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(71, 8, 2, 3, LocalizedString.FromResource(() => ItemNames.AtlansArmor), 80, 47, 150, CharacterClasses.AllMGs);
        this.CreateLuckyArmor(72, 8, 2, 3, LocalizedString.FromResource(() => ItemNames.StormJahadArmor), 82, 43, 150, CharacterClasses.AllFighters);

        // Pants:
        this.CreateLuckyArmor(62, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.ScalePants), 25, 18, 1, CharacterClasses.AllKnights | CharacterClasses.AllMGs | CharacterClasses.AllLords | CharacterClasses.AllFighters);
        this.CreateLuckyArmor(63, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkPants), 18, 13, 1, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(64, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxPants), 34, 19, 1, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(65, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.ViolentWindPants), 30, 20, 1, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(66, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantinePants), 70, 31, 150, CharacterClasses.AllLords);
        this.CreateLuckyArmor(67, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonPants), 55, 28, 150, CharacterClasses.AllKnights | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(68, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianPants), 54, 30, 150, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(69, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryPants), 53, 25, 150, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(70, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingPants), 53, 26, 150, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(71, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.AtlansPants), 74, 39, 150, CharacterClasses.AllMGs);
        this.CreateLuckyArmor(72, 9, 2, 2, LocalizedString.FromResource(() => ItemNames.StormJahadPants), 78, 33, 150, CharacterClasses.AllFighters);

        // Gloves:
        this.CreateLuckyArmor(62, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleGloves), 22, 10, 1, CharacterClasses.AllKnights | CharacterClasses.AllMGs | CharacterClasses.AllLords | CharacterClasses.AllFighters);
        this.CreateLuckyArmor(63, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkGloves), 14, 6, 1, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(64, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxGloves), 28, 12, 1, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(65, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.ViolentWindGloves), 24, 13, 1, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(66, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantineGloves), 57, 20, 150, CharacterClasses.AllLords);
        this.CreateLuckyArmor(67, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonGloves), 52, 19, 150, CharacterClasses.AllKnights | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(68, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianGloves), 50, 20, 150, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(69, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryGloves), 44, 16, 150, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(70, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingGloves), 44, 18, 150, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(71, 10, 2, 2, LocalizedString.FromResource(() => ItemNames.AtlansGloves), 70, 25, 150, CharacterClasses.AllMGs);

        // Boots:
        this.CreateLuckyArmor(62, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.ScaleBoots), 22, 12, 1, CharacterClasses.AllKnights | CharacterClasses.AllMGs | CharacterClasses.AllLords | CharacterClasses.AllFighters);
        this.CreateLuckyArmor(63, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.SilkBoots), 15, 7, 1, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(64, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.SphinxBoots), 30, 13, 1, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(65, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.ViolentWindBoots), 26, 15, 1, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(66, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.AdamantineBoots), 60, 25, 150, CharacterClasses.AllLords);
        this.CreateLuckyArmor(67, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.DragonBoots), 54, 20, 150, CharacterClasses.AllKnights | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(68, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.GuardianBoots), 52, 23, 150, CharacterClasses.AllElfs);
        this.CreateLuckyArmor(69, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.LegendaryBoots), 46, 17, 150, CharacterClasses.AllWizards | CharacterClasses.AllMGs);
        this.CreateLuckyArmor(70, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.RedWingBoots), 46, 17, 150, CharacterClasses.AllSummoners);
        this.CreateLuckyArmor(71, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.AtlansBoots), 72, 27, 150, CharacterClasses.AllMGs);
        this.CreateLuckyArmor(72, 11, 2, 2, LocalizedString.FromResource(() => ItemNames.StormJahadBoots), 62, 20, 150, CharacterClasses.AllFighters);
    }

    private void CreateTickets()
    {
        this.CreateSimpleItem(13, 135, LocalizedString.FromResource(() => ItemNames.FirstLuckyArmorTicket));
        this.CreateSimpleItem(13, 136, LocalizedString.FromResource(() => ItemNames.FirstLuckyPantsTicket));
        this.CreateSimpleItem(13, 137, LocalizedString.FromResource(() => ItemNames.FirstLuckyHelmTicket));
        this.CreateSimpleItem(13, 138, LocalizedString.FromResource(() => ItemNames.FirstLuckyGlovesTicket));
        this.CreateSimpleItem(13, 139, LocalizedString.FromResource(() => ItemNames.FirstLuckyBootsTicket));
        this.CreateSimpleItem(13, 140, LocalizedString.FromResource(() => ItemNames.SecondLuckyArmorTicket));
        this.CreateSimpleItem(13, 141, LocalizedString.FromResource(() => ItemNames.SecondLuckyPantsTicket));
        this.CreateSimpleItem(13, 142, LocalizedString.FromResource(() => ItemNames.SecondLuckyHelmTicket));
        this.CreateSimpleItem(13, 143, LocalizedString.FromResource(() => ItemNames.SecondLuckyGlovesTicket));
        this.CreateSimpleItem(13, 144, LocalizedString.FromResource(() => ItemNames.SecondLuckyBootsTicket));
    }

    private void CreateJewels()
    {
        this.CreateSimpleItem(14, 160, LocalizedString.FromResource(() => ItemNames.JewelOfExtension));
        this.CreateSimpleItem(14, 161, LocalizedString.FromResource(() => ItemNames.JewelOfElevation));
    }

    private bool Exists(byte group, short number) => this.GameConfiguration.Items.Any(item => item.Group == group && item.Number == number);

    private void CreateSimpleItem(byte group, short number, LocalizedString name)
    {
        if (this.Exists(group, number))
        {
            return;
        }

        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = 1;
        item.Height = 1;
        item.Durability = 1;
        item.DropsFromMonsters = false;
        item.SetGuid(item.Group, item.Number);
        this._createdItems.Add(item);
    }

    private void CreateLuckyArmor(short number, byte group, byte width, byte height, LocalizedString name, byte dropLevel, int defense, int levelRequirement, CharacterClasses qualifiedClasses)
    {
        if (this.Exists(group, number))
        {
            return;
        }

        var armor = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(armor);
        armor.Group = group;
        armor.Number = number;
        armor.Width = width;
        armor.Height = height;
        armor.Name = name;
        armor.DropLevel = dropLevel;
        armor.DropsFromMonsters = false;
        armor.ExcellentRaisesLevel = false;
        armor.Durability = byte.MaxValue;
        var slot = group - 5;
        armor.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(slot));
        this.CreateItemRequirementIfNeeded(armor, Stats.Level, levelRequirement);
        armor.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.DefenseBase, defense, AggregateType.AddRaw));

        foreach (var characterClass in this.GameConfiguration.DetermineCharacterClasses(qualifiedClasses))
        {
            armor.QualifiedCharacters.Add(characterClass);
        }

        if (this.GameConfiguration.ItemOptions.FirstOrDefault(o => o.Name.ValueInNeutralLanguage == HarmonyOptions.DefenseOptionsName) is { } harmonyOptions)
        {
            armor.PossibleItemOptions.Add(harmonyOptions);
        }

        armor.SetGuid(armor.Group, armor.Number);
        this._createdItems.Add(armor);
    }
}
