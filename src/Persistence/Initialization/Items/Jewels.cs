// <copyright file="Jewels.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Class which contains item definitions for jewels.
    /// </summary>
    public static class Jewels
    {
        /// <summary>
        /// Creates an item definition for the Jewel of Bless.
        /// </summary>
        /// <returns>Item definition for the Jewel of Bless</returns>
        public static ItemDefinition Bless()
        {
            return new ItemDefinition
            {
                Name = "Jewel of Bless",
                Number = 13,
                Group = 14,
                DropsFromMonsters = true,
                DropLevel = 25,
                ConsumeHandlerClass = typeof(OpenMU.GameLogic.PlayerActions.ItemConsumeActions.BlessJewelConsumeHandler).FullName,
                Durability = 1,
                Width = 1,
                Height = 1,
            };
        }
    }
}
