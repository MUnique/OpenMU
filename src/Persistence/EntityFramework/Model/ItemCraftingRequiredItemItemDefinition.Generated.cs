// <copyright file="ItemCraftingRequiredItemItemDefinition.Generated.cs" company="MUnique">
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

    [Table(nameof(ItemCraftingRequiredItemItemDefinition), Schema = SchemaNames.Configuration)]
    internal partial class ItemCraftingRequiredItemItemDefinition
    {
        public Guid ItemCraftingRequiredItemId { get; set; }
        public ItemCraftingRequiredItem ItemCraftingRequiredItem { get; set; }

        public Guid ItemDefinitionId { get; set; }
        public ItemDefinition ItemDefinition { get; set; }
    }

    internal partial class ItemCraftingRequiredItem
    {
        public ICollection<ItemCraftingRequiredItemItemDefinition> JoinedPossibleItems { get; } = new EntityFramework.List<ItemCraftingRequiredItemItemDefinition>();
    }
}