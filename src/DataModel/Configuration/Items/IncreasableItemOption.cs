// <copyright file="IncreasableItemOption.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration.Items
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines an item option which can be increased.
    /// </summary>
    public class IncreasableItemOption : ItemOption
    {
        /// <summary>
        /// Gets or sets the level dependent options.
        /// </summary>
        public virtual ICollection<ItemOptionOfLevel> LevelDependentOptions { get; protected set; }
    }
}
