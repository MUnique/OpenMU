// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

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
        // Devil Square:
        this.CreateEventItem(17, 14, 1, 1, "Devil's Eye", 4, true,  2, 36, 47, 60);
        this.CreateEventItem(18, 14, 1, 1, "Devil's Key", 4, true, 2, 36, 47, 60);
        this.CreateEventItem(19, 14, 1, 1, "Devil's Invitation", 4, false);
    }

    private void CreateEventItem(byte number, byte group, byte width, byte height, string name, byte maxItemLevel, bool dropsFromMonster, params byte[] dropLevels)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Group = group;
        item.Number = number;
        item.Name = name;
        item.Width = width;
        item.Height = height;
        item.Durability = 1;
        item.MaximumItemLevel = maxItemLevel;
        item.DropsFromMonsters = dropsFromMonster;
        item.SetGuid(item.Group, item.Number);
        if (dropLevels.Length == 1)
        {
            item.DropLevel = dropLevels.First();
            return;
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
    }
}