// <copyright file="MoneyItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A money item which is not persistent.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.DataModel.Entities.Item" />
    public sealed class MoneyItem : Item
    {
        /// <summary>
        /// Gets or sets the amount of money.
        /// </summary>
        public uint Amount { get; set; }
    }
}
