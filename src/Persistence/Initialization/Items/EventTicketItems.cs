// <copyright file="EventTicketItems.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
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
            this.CreateEventItem(16, 13, 1, 2, true, "Scroll of Archangel");
            this.CreateEventItem(17, 13, 1, 2, true, "Blood Bone");
            this.CreateEventItem(18, 13, 2, 2, false, "Invisibility Cloak");
            this.CreateEventItem(19, 13, 1, 2, false, "Weapon of Archangel");

            // Chaos Castle:
            this.CreateEventItem(29, 13, 2, 2, false, "Armor of Guardsman");

            // Illusion Temple:
            this.CreateEventItem(49, 13, 1, 1, true, "Old Scroll");
            this.CreateEventItem(50, 13, 1, 2, true, "Illusion Sorcerer Covenant");
            this.CreateEventItem(51, 13, 2, 2, false, "Scroll of Blood");

            // Devil Square:
            this.CreateEventItem(17, 14, 1, 1, true, "Devil's Eye");
            this.CreateEventItem(18, 14, 1, 1, true, "Devil's Key");
            this.CreateEventItem(19, 14, 1, 1, false, "Devil's Invitation");
        }

        private ItemDefinition CreateEventItem(byte number, byte group, byte width, byte height, bool dropsFromMonsters, string name)
        {
            var item = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(item);
            item.Group = group;
            item.Number = number;
            item.DropsFromMonsters = dropsFromMonsters;
            item.Name = name;
            item.Width = width;
            item.Height = height;
            item.Durability = 1;
            return item;
        }
    }
}
