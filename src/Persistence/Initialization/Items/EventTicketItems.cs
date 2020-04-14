// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.Persistence.Initialization.Maps;

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
            this.CreateEventItem(16, 13, 1, 2, "Scroll of Archangel", 2, 32, 45, 57, 68, 76, 84, 95);
            this.CreateEventItem(17, 13, 1, 2, "Blood Bone", 2, 32, 45, 57, 68, 76, 84, 95);
            this.CreateEventItem(18, 13, 2, 2, "Invisibility Cloak");
            this.CreateEventItem(19, 13, 1, 2, "Weapon of Archangel");

            // Chaos Castle:
            this.CreateEventItem(29, 13, 2, 2, "Armor of Guardsman");

            // Illusion Temple:
            this.CreateEventItem(49, 13, 1, 1, "Old Scroll", 66, 72, 78, 84, 90, 96);
            this.CreateEventItem(50, 13, 1, 2, "Illusion Sorcerer Covenant", 70, 76, 82, 88, 94, 100);
            this.CreateEventItem(51, 13, 2, 2, "Scroll of Blood");

            // Devil Square:
            this.CreateEventItem(17, 14, 1, 1, "Devil's Eye", 2, 36, 47, 60, 70, 80, 90);
            this.CreateEventItem(18, 14, 1, 1, "Devil's Key", 2, 36, 47, 60, 70, 80, 90);
            this.CreateEventItem(19, 14, 1, 1, "Devil's Invitation");
        }

        private void CreateEventItem(byte number, byte group, byte width, byte height, string name, params byte[] dropLevels)
        {
            var item = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(item);
            item.Group = group;
            item.Number = number;
            item.Name = name;
            item.Width = width;
            item.Height = height;
            item.Durability = 1;

            byte itemLevel = 1;
            DropItemGroup previousGroup = null;
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
}
