// <copyright file="ItemSlotType.Generated.cs" company="MUnique">
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
    
    /// <summary>
    /// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType"/>.
    /// </summary>
    [Table(nameof(ItemSlotType), Schema = SchemaNames.Configuration)]
    internal partial class ItemSlotType : MUnique.OpenMU.DataModel.Configuration.Items.ItemSlotType, IIdentifiable
    {
        
        
        /// <summary>
        /// Gets or sets the identifier of this instance.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets the raw string of <see cref="ItemSlots" />.
        /// </summary>
        [Column(nameof(ItemSlots))]
        [Newtonsoft.Json.JsonProperty(nameof(ItemSlots))]
        [System.Text.Json.Serialization.JsonPropertyName("itemSlots")]
        public string RawItemSlots { get; set; }
        
        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        [NotMapped]
        public override ICollection<System.Int32> ItemSlots
        {
            get => base.ItemSlots ??= new CollectionToStringAdapter<System.Int32>(this.RawItemSlots, newString => this.RawItemSlots = newString);
            protected set
            {
                this.ItemSlots.Clear();
                foreach (var item in value)
                {
                    this.ItemSlots.Add(item);
                }
            }
        }


        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var baseObject = obj as IIdentifiable;
            if (baseObject != null)
            {
                return baseObject.Id == this.Id;
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        
    }
}