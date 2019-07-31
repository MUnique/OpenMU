// <copyright file="DropItemGroupExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using Moq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Some extensions methods for convenience at testing item drops.
    /// </summary>
    internal static class DropItemGroupExtensions
    {
        /// <summary>
        /// Adds the basic drop item groups to the player.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <returns>The same player.</returns>
        public static Player WithBasicDropItemGroups(this Player player)
        {
            player.CurrentMap.Definition.DropItemGroups.AddBasicDropItemGroups();
            return player;
        }

        /// <summary>
        /// Adds the basic drop item groups.
        /// </summary>
        /// <param name="itemGroups">The item groups.</param>
        public static void AddBasicDropItemGroups(this ICollection<DropItemGroup> itemGroups)
        {
            itemGroups.Add(1, SpecialItemType.RandomItem, true);
            itemGroups.Add(1000, SpecialItemType.Excellent, true);
            itemGroups.Add(3000, SpecialItemType.Money, true);
        }

        /// <summary>
        /// Adds a new drop item group with the specified data.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="chance">The chance.</param>
        /// <param name="itemType">Type of the item.</param>
        /// <param name="addItem">if set to <c>true</c>, it adds a test item to the possible item list.</param>
        /// <returns>The drop item group which has been added to the list.</returns>
        public static DropItemGroup Add(this ICollection<DropItemGroup> list, int chance, SpecialItemType itemType, bool addItem)
        {
            var dropItemGroup = new Mock<DropItemGroup>();
            dropItemGroup.SetupAllProperties();
            dropItemGroup.Object.Chance = chance / 10000.0;
            dropItemGroup.Object.ItemType = itemType;
            var itemList = new List<ItemDefinition>();
            dropItemGroup.Setup(g => g.PossibleItems).Returns(itemList);
            if (addItem)
            {
                var itemDefinition = new Mock<ItemDefinition>();
                itemDefinition.SetupAllProperties();
                itemDefinition.Object.DropsFromMonsters = true;
                itemDefinition.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
                itemDefinition.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
                itemList.Add(itemDefinition.Object);
            }

            list.Add(dropItemGroup.Object);

            return dropItemGroup.Object;
        }
    }
}