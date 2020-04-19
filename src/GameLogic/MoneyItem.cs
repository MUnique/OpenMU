// <copyright file="MoneyItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A temporary item which is not persistent (yet).
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.DataModel.Entities.Item" />
    public sealed class MoneyItem : Item
    {
        /// <summary>
        /// Gets or sets the amount of money.
        /// </summary>
        public uint Amount { get; set; }

        /// <summary>
        /// This item has no persistence.
        /// </summary>
        /// <param name="persistenceContext">The persistence context where the item should be added.</param>
        /// <returns>The persistent item.</returns>
        public Item MakePersistent(IContext persistenceContext)
        {
            return this;
        }
    }
}
