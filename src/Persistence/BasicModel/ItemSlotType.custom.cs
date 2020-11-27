// <copyright file="ItemSlotType.custom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using System.Collections.Generic;

    /// <summary>
    /// A plain implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType"/>.
    /// </summary>
    public partial class ItemSlotType
    {
        /// <inheritdoc />
        public override ICollection<int> ItemSlots => base.ItemSlots ?? (base.ItemSlots = new List<int>());
    }
}
