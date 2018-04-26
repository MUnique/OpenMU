// <copyright file="TemporaryItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A temporary item which is not persistent (yet).
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.DataModel.Entities.Item" />
    public sealed class TemporaryItem : Item
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporaryItem"/> class.
        /// </summary>
        public TemporaryItem()
        {
            this.ItemOptions = new List<ItemOptionLink>();
            this.ItemSetGroups = new List<ItemSetGroup>();
        }

        /// <summary>
        /// Makes this item persistent.
        /// </summary>
        /// <param name="persistenceContext">The persistence context where the item should be added.</param>
        /// <returns>The persistent item.</returns>
        public Item MakePersistent(IContext persistenceContext)
        {
            var persistentItem = persistenceContext.CreateNew<Item>();
            persistentItem.AssignValues(this);
            return persistentItem;
        }
    }
}
