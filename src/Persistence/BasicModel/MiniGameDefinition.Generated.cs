// <copyright file="MiniGameDefinition.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Persistence.Json;
    
    /// <summary>
    /// A plain implementation of <see cref="MiniGameDefinition"/>.
    /// </summary>
    public partial class MiniGameDefinition : MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition, IIdentifiable, IConvertibleTo<MiniGameDefinition>
    {
        
        /// <summary>
        /// Gets or sets the identifier of this instance.
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Gets the raw collection of <see cref="Rewards" />.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("rewards")]
        [System.Text.Json.Serialization.JsonPropertyName("rewards")]
        public ICollection<MiniGameReward> RawRewards { get; } = new List<MiniGameReward>();
        
        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public override ICollection<MUnique.OpenMU.DataModel.Configuration.MiniGameReward> Rewards
        {
            get => base.Rewards ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.MiniGameReward, MiniGameReward>(this.RawRewards);
            protected set
            {
                this.Rewards.Clear();
                foreach (var item in value)
                {
                    this.Rewards.Add(item);
                }
            }
        }

        /// <summary>
        /// Gets the raw object of <see cref="Entrance" />.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("entrance")]
        [System.Text.Json.Serialization.JsonPropertyName("entrance")]
        public ExitGate RawEntrance
        {
            get => base.Entrance as ExitGate;
            set => base.Entrance = value;
        }

        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public override MUnique.OpenMU.DataModel.Configuration.ExitGate Entrance
        {
            get => base.Entrance;
            set => base.Entrance = value;
        }

        /// <summary>
        /// Gets the raw object of <see cref="TicketItem" />.
        /// </summary>
        [Newtonsoft.Json.JsonProperty("ticketItem")]
        [System.Text.Json.Serialization.JsonPropertyName("ticketItem")]
        public ItemDefinition RawTicketItem
        {
            get => base.TicketItem as ItemDefinition;
            set => base.TicketItem = value;
        }

        /// <inheritdoc/>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public override MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition TicketItem
        {
            get => base.TicketItem;
            set => base.TicketItem = value;
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

        /// <inheritdoc/>
        public MiniGameDefinition Convert() => this;
    }
}