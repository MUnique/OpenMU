// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

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
        this.CreateEventItem(16, 13, 1, 2, "Scroll of Archangel", true, 8, 2, 32, 45, 57, 68, 76, 84, 95);
        this.CreateEventItem(17, 13, 1, 2, "Blood Bone", true, 8, 2, 32, 45, 57, 68, 76, 84, 95);
        this.CreateEventItem(18, 13, 2, 2, "Invisibility Cloak", false, 8);
        this.CreateEventItem(19, 13, 1, 2, "Weapon of Archangel", false);

        // Chaos Castle:
        this.CreateEventItem(29, 13, 2, 2, "Armor of Guardsman", false);

        // Illusion Temple:
        this.CreateEventItem(49, 13, 1, 1, "Old Scroll", true, 6, 66, 72, 78, 84, 90, 96);
        this.CreateEventItem(50, 13, 1, 2, "Illusion Sorcerer Covenant", true, 6, 70, 76, 82, 88, 94, 100);
        this.CreateEventItem(51, 13, 2, 2, "Scroll of Blood", false, 6);

        // Devil Square:
        this.CreateEventItem(17, 14, 1, 1, "Devil's Eye", true, 7, 2, 36, 47, 60, 70, 80, 90);
        this.CreateEventItem(18, 14, 1, 1, "Devil's Key", true, 7, 2, 36, 47, 60, 70, 80, 90);
        this.CreateEventItem(19, 14, 1, 1, "Devil's Invitation", false, 7);

        // Imperial Guardian
        var scrapOfPaper = this.CreateEventItem(101, 14, 1, 1, "Suspicious Scrap of Paper", true, 0, 32);
        scrapOfPaper.Durability = 5;
        this.CreateEventItem(102, 14, 1, 1, "Gaion's Order", false);
        this.CreateEventItem(103, 14, 1, 1, "First Secromicon Fragment", false);
        this.CreateEventItem(104, 14, 1, 1, "Second Secromicon Fragment", false);
        this.CreateEventItem(105, 14, 1, 1, "Third Secromicon Fragment", false);
        this.CreateEventItem(106, 14, 1, 1, "Fourth Secromicon Fragment", false);
        this.CreateEventItem(107, 14, 1, 1, "Fifth Secromicon Fragment", false);
        this.CreateEventItem(108, 14, 1, 1, "Sixth Secromicon Fragment", false);
        this.CreateEventItem(109, 14, 1, 1, "Complete Secromicon", false);
    }

    private ItemDefinition CreateEventItem(byte number, byte group, byte width, byte height, string name, bool dropsFromMonsters, byte maxItemLevel = 0, params byte[] dropLevels)
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