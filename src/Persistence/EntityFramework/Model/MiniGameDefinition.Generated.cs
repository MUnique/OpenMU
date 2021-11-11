// <copyright file="MiniGameDefinition.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.EntityFramework.Model;

using System.ComponentModel.DataAnnotations.Schema;
using MUnique.OpenMU.Persistence;

/// <summary>
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition"/>.
/// </summary>
[Table(nameof(MiniGameDefinition), Schema = SchemaNames.Configuration)]
internal partial class MiniGameDefinition : MUnique.OpenMU.DataModel.Configuration.MiniGameDefinition, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw collection of <see cref="Rewards" />.
    /// </summary>
    public ICollection<MiniGameReward> RawRewards { get; } = new EntityFramework.List<MiniGameReward>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.MiniGameReward> Rewards => base.Rewards ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.MiniGameReward, MiniGameReward>(this.RawRewards);

    /// <summary>
    /// Gets the raw collection of <see cref="SpawnWaves" />.
    /// </summary>
    public ICollection<MiniGameSpawnWave> RawSpawnWaves { get; } = new EntityFramework.List<MiniGameSpawnWave>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Configuration.MiniGameSpawnWave> SpawnWaves => base.SpawnWaves ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Configuration.MiniGameSpawnWave, MiniGameSpawnWave>(this.RawSpawnWaves);

    /// <summary>
    /// Gets or sets the identifier of <see cref="Entrance"/>.
    /// </summary>
    public Guid? EntranceId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Entrance" />.
    /// </summary>
    [ForeignKey(nameof(EntranceId))]
    public ExitGate RawEntrance
    {
        get => base.Entrance as ExitGate;
        set => base.Entrance = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.ExitGate Entrance
    {
        get => base.Entrance;set
        {
            base.Entrance = value;
            this.EntranceId = this.RawEntrance?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="TicketItem"/>.
    /// </summary>
    public Guid? TicketItemId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="TicketItem" />.
    /// </summary>
    [ForeignKey(nameof(TicketItemId))]
    public ItemDefinition RawTicketItem
    {
        get => base.TicketItem as ItemDefinition;
        set => base.TicketItem = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition TicketItem
    {
        get => base.TicketItem;set
        {
            base.TicketItem = value;
            this.TicketItemId = this.RawTicketItem?.Id;
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
