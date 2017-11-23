// <copyright file="Potions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Class which contains item definitions for jewels.
    /// </summary>
    public static class Potions
    {
        /// <summary>
        /// Gets the apple definition.
        /// </summary>
        public static ItemDefinition Apple
        {
            get
            {
                // TODO!!!
                return new ItemDefinition
                {
                    Name = "Apple",
                    Number = 0,
                    Group = 14,
                    DropsFromMonsters = true,
                    DropLevel = 1,
                    ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.AppleConsumeHandler).FullName,
                    Durability = 1,
                    Value = 5,
                    Width = 1,
                    Height = 1,
                };
            }
        }

        /// <summary>
        /// Gets the small healing potion definition.
        /// </summary>
        public static ItemDefinition SmallHealingPotion
        {
            get
            {
                return new ItemDefinition
                {
                    Name = "Small Healing Potion",
                    Number = 1,
                    Group = 14,
                    DropsFromMonsters = true,
                    DropLevel = 10,
                    ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.SmallHealthPotionConsumeHandler).FullName,
                    Durability = 1,
                    Value = 10,
                    Width = 1,
                    Height = 1,
                };
            }
        }

        /// <summary>
        /// Gets the medium healing potion definition.
        /// </summary>
        public static ItemDefinition MediumHealingPotion
        {
            get
            {
                return new ItemDefinition
                {
                    Name = "Medium Healing Potion",
                    Number = 2,
                    Group = 14,
                    DropsFromMonsters = true,
                    DropLevel = 25,
                    ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.MiddleHealthPotionConsumeHandler).FullName,
                    Durability = 1,
                    Value = 20,
                    Width = 1,
                    Height = 1,
                };
            }
        }

        /// <summary>
        /// Gets the large healing potion definition.
        /// </summary>
        public static ItemDefinition LargeHealingPotion
        {
            get
            {
                return new ItemDefinition
                {
                    Name = "Large Healing Potion",
                    Number = 3,
                    Group = 14,
                    DropsFromMonsters = true,
                    DropLevel = 40,
                    ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BigHealthPotionConsumeHandler).FullName,
                    Durability = 1,
                    Value = 30,
                    Width = 1,
                    Height = 1,
                };
            }
        }

        /// <summary>
        /// Gets all potion item definitions.
        /// </summary>
        /// <returns>All potion item definitions.</returns>
        public static IEnumerable<ItemDefinition> GetAll()
        {
            yield return Apple;
            yield return SmallHealingPotion;
            yield return MediumHealingPotion;
            yield return LargeHealingPotion;

            // etc...
        }
    }
}
