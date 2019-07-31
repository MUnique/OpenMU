// <copyright file="MixResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    /// <summary>
    /// Defines what should happen with the required input items when the item crafting finished.
    /// </summary>
    public enum MixResult
    {
        /// <summary>
        /// The item will disappear.
        /// </summary>
        Disappear = 0,

        /// <summary>
        /// The item will stay as is.
        /// </summary>
        StaysAsIs = 1,

        /// <summary>
        /// The item will be downgraded to level 0.
        /// </summary>
        DowngradedTo0 = 3,

        /// <summary>
        /// The item will be downgraded to a random level.
        /// </summary>
        DowngradedRandom = 4,
    }
}
