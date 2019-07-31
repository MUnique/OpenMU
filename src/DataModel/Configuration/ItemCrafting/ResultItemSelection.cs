// <copyright file="ResultItemSelection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.ItemCrafting
{
    /// <summary>
    /// Defines the result item selection.
    /// </summary>
    public enum ResultItemSelection
    {
        /// <summary>
        /// One random item is selected as result from the result items.
        /// </summary>
        Any = 0,

        /// <summary>
        /// All items are selected as result from the result items.
        /// </summary>
        All = 1,
    }
}
