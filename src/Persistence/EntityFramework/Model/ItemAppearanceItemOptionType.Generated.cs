// <copyright file="ItemAppearanceItemOptionType.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.EntityFramework;

    [Table(nameof(ItemAppearanceItemOptionType), Schema = SchemaNames.AccountData)]
    internal partial class ItemAppearanceItemOptionType
    {
        public Guid ItemAppearanceId { get; set; }
        public ItemAppearance ItemAppearance { get; set; }

        public Guid ItemOptionTypeId { get; set; }
        public ItemOptionType ItemOptionType { get; set; }
    }

    internal partial class ItemAppearance
    {
        public ICollection<ItemAppearanceItemOptionType> JoinedVisibleOptions { get; } = new EntityFramework.List<ItemAppearanceItemOptionType>();
    }
}