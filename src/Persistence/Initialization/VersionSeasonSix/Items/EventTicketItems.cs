// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Initializer for event related items.
/// </summary>
internal class EventTicketItems : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventTicketItems"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public EventTicketItems(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        // Blood Castle:
        this.CreateEventItem(16, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.ScrollOfArchangel), false, 8, 2, 32, 45, 57, 68, 76, 84, 95);
        this.CreateEventItem(17, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.BloodBone), false, 8, 2, 32, 45, 57, 68, 76, 84, 95);
        this.CreateEventItem(18, 13, 2, 2, LocalizedString.FromResource(() => ItemNames.InvisibilityCloak), false, 8);
        this.CreateEventItem(19, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.WeaponOfArchangel), false);

        // Chaos Castle:
        this.CreateEventItem(29, 13, 2, 2, LocalizedString.FromResource(() => ItemNames.ArmorOfGuardsman), false);

        // Illusion Temple:
        this.CreateEventItem(49, 13, 1, 1, LocalizedString.FromResource(() => ItemNames.OldScroll), false, 6, 66, 72, 78, 84, 90, 96);
        this.CreateEventItem(51, 13, 1, 2, LocalizedString.FromResource(() => ItemNames.IllusionSorcererCovenant), false, 6, 70, 76, 82, 88, 94, 100);
        this.CreateEventItem(50, 13, 2, 2, LocalizedString.FromResource(() => ItemNames.ScrollOfBlood), false, 6);

        // The sacred relic, which the client knows as ITEM_CURSED_CASTLE_WATER.
        this.CreateEventItem(64, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.CursedCastleWater), false);

        // Doppelganger:
        if (this.CreateDoppelgangerItems() is { } signOfDimensionsDropGroup)
        {
            BaseMapInitializer.RegisterDefaultDropItemGroup(signOfDimensionsDropGroup);
        }

        // Devil Square:
        this.CreateEventItem(17, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.DevilSEye), false, 7, 2, 36, 47, 60, 70, 80, 90);
        this.CreateEventItem(18, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.DevilSKey), false, 7, 2, 36, 47, 60, 70, 80, 90);
        this.CreateEventItem(19, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.DevilSInvitation), false, 7);

        // Free tickets of the in-game shop, which allow to enter an event of any level:
        this.CreateFreeTicketItems();

        // Imperial Guardian
        var scrapOfPaper = this.CreateEventItem(101, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.SuspiciousScrapOfPaper), false, 0, 32);
        scrapOfPaper.Durability = 5;
        this.CreateEventItem(102, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.GaionSOrder), false);
        this.CreateEventItem(103, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.FirstSecromiconFragment), false);
        this.CreateEventItem(104, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.SecondSecromiconFragment), false);
        this.CreateEventItem(105, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.ThirdSecromiconFragment), false);
        this.CreateEventItem(106, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.FourthSecromiconFragment), false);
        this.CreateEventItem(107, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.FifthSecromiconFragment), false);
        this.CreateEventItem(108, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.SixthSecromiconFragment), false);
        this.CreateEventItem(109, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.CompleteSecromicon), false);
        if (this.CreateSuspiciousScrapOfPaperDropGroup() is { } scrapOfPaperDropGroup)
        {
            BaseMapInitializer.RegisterDefaultDropItemGroup(scrapOfPaperDropGroup);
        }
    }

    /// <summary>
    /// Creates the drop item group of the suspicious scrap of paper, of which five transform into a
    /// Gaion's Order, the ticket of the imperial guardian event.
    /// </summary>
    /// <returns>The drop item group, if it was created.</returns>
    internal DropItemGroup? CreateSuspiciousScrapOfPaperDropGroup()
    {
        var scrapOfPaper = this.GameConfiguration.Items.FirstOrDefault(item => item is { Group: 14, Number: 101 });
        var id = GuidHelper.CreateGuid<DropItemGroup>(14, 101);
        if (scrapOfPaper is null || this.GameConfiguration.DropItemGroups.Any(group => group.GetId() == id))
        {
            return null;
        }

        var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
        dropItemGroup.SetGuid(14, 101);
        dropItemGroup.Chance = 0.001;
        dropItemGroup.Description = LocalizedString.FromResource(() => ItemNames.SuspiciousScrapOfPaper);
        dropItemGroup.MinimumMonsterLevel = 32;
        dropItemGroup.PossibleItems.Add(scrapOfPaper);
        this.GameConfiguration.DropItemGroups.Add(dropItemGroup);
        return dropItemGroup;
    }

    /// <summary>
    /// Creates the items of the doppelganger event, which don't exist yet:
    /// The Mirror of Dimensions and the Doppelganger Free Ticket, which allow to enter the event,
    /// and the Sign of Dimensions, of which five transform into a Mirror of Dimensions.
    /// </summary>
    /// <returns>The drop item group of the Sign of Dimensions, if it was created.</returns>
    internal DropItemGroup? CreateDoppelgangerItems()
    {
        if (!this.GameConfiguration.Items.Any(item => item is { Group: 14, Number: 111 }))
        {
            this.CreateEventItem(111, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.MirrorOfDimensions), false);
        }

        if (!this.GameConfiguration.Items.Any(item => item is { Group: 13, Number: 125 }))
        {
            this.CreateEventItem(125, 13, 1, 1, LocalizedString.FromResource(() => ItemNames.DoppelgangerFreeTicket), false);
        }

        if (this.GameConfiguration.Items.Any(item => item is { Group: 14, Number: 110 }))
        {
            return null;
        }

        // It's stackable up to five pieces, which transform into a Mirror of Dimensions.
        var signOfDimensions = this.CreateEventItem(110, 14, 1, 1, LocalizedString.FromResource(() => ItemNames.SignOfDimensions), true);
        signOfDimensions.Durability = 5;

        var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
        dropItemGroup.SetGuid(14, 110);
        dropItemGroup.Chance = 0.001;
        dropItemGroup.Description = LocalizedString.FromResource(() => DropGroupDescriptions.SignOfDimensions);
        dropItemGroup.MinimumMonsterLevel = 32;
        dropItemGroup.PossibleItems.Add(signOfDimensions);
        this.GameConfiguration.DropItemGroups.Add(dropItemGroup);
        return dropItemGroup;
    }

    /// <summary>
    /// Creates the free tickets of the in-game shop, which don't exist yet. They allow to enter
    /// Devil Square, Blood Castle, Illusion Temple and Chaos Castle of any level, instead of their
    /// regular tickets. Their durability is the number of entries.
    /// </summary>
    /// <returns>The created items.</returns>
    internal IReadOnlyList<ItemDefinition> CreateFreeTicketItems()
    {
        var createdItems = new List<ItemDefinition>();
        void CreateIfMissing(byte number, LocalizedString name)
        {
            if (!this.GameConfiguration.Items.Any(item => item.Group == 13 && item.Number == number))
            {
                createdItems.Add(this.CreateEventItem(number, 13, 1, 1, name, false));
            }
        }

        CreateIfMissing(46, LocalizedString.FromResource(() => ItemNames.DevilSquareFreeTicket));
        CreateIfMissing(47, LocalizedString.FromResource(() => ItemNames.BloodCastleFreeTicket));
        CreateIfMissing(61, LocalizedString.FromResource(() => ItemNames.IllusionTempleFreeTicket));
        CreateIfMissing(121, LocalizedString.FromResource(() => ItemNames.ChaosCastleFreeTicket));
        return createdItems;
    }

    private ItemDefinition CreateEventItem(byte number, byte group, byte width, byte height, LocalizedString name, bool dropsFromMonsters, byte maxItemLevel = 0, params byte[] dropLevels)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = width;
        item.Height = height;
        item.Durability = 1;
        item.DropsFromMonsters = dropsFromMonsters;
        item.MaximumItemLevel = maxItemLevel;
        item.SetGuid(item.Group, item.Number);
        if (dropLevels.Length == 1)
        {
            item.DropLevel = dropLevels.First();
            return item;
        }

        byte itemLevel = 1;
        DropItemGroup? previousGroup = null;
        foreach (var dropLevel in dropLevels)
        {
            if (previousGroup is { })
            {
                previousGroup.MaximumMonsterLevel = (byte)(dropLevel - 1);
            }

            var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
            dropItemGroup.ItemLevel = itemLevel;
            dropItemGroup.Chance = 0.01;
            dropItemGroup.Description = name + "+" + itemLevel;
            dropItemGroup.PossibleItems.Add(item);
            dropItemGroup.MinimumMonsterLevel = dropLevel;
            this.GameConfiguration.DropItemGroups.Add(dropItemGroup);
            BaseMapInitializer.RegisterDefaultDropItemGroup(dropItemGroup);

            previousGroup = dropItemGroup;
            itemLevel++;
        }

        return item;
    }
}