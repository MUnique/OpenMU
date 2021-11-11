// <copyright file="Guild.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Entities.Guild"/>.
/// </summary>
[Table(nameof(Guild), Schema = SchemaNames.AccountData)]
internal partial class Guild : MUnique.OpenMU.DataModel.Entities.Guild, IIdentifiable
{
    
    
    
    /// <summary>
    /// Gets the raw collection of <see cref="Members" />.
    /// </summary>
    public ICollection<GuildMember> RawMembers { get; } = new EntityFramework.List<GuildMember>();
    
    /// <inheritdoc/>
    [NotMapped]
    public override ICollection<MUnique.OpenMU.DataModel.Entities.GuildMember> Members => base.Members ??= new CollectionAdapter<MUnique.OpenMU.DataModel.Entities.GuildMember, GuildMember>(this.RawMembers);

    /// <summary>
    /// Gets or sets the identifier of <see cref="Hostility"/>.
    /// </summary>
    public Guid? HostilityId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Hostility" />.
    /// </summary>
    [ForeignKey(nameof(HostilityId))]
    public Guild RawHostility
    {
        get => base.Hostility as Guild;
        set => base.Hostility = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.Interfaces.Guild Hostility
    {
        get => base.Hostility;set
        {
            base.Hostility = value;
            this.HostilityId = this.RawHostility?.Id;
        }
    }

    /// <summary>
    /// Gets or sets the identifier of <see cref="AllianceGuild"/>.
    /// </summary>
    public Guid? AllianceGuildId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="AllianceGuild" />.
    /// </summary>
    [ForeignKey(nameof(AllianceGuildId))]
    public Guild RawAllianceGuild
    {
        get => base.AllianceGuild as Guild;
        set => base.AllianceGuild = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.Interfaces.Guild AllianceGuild
    {
        get => base.AllianceGuild;set
        {
            base.AllianceGuild = value;
            this.AllianceGuildId = this.RawAllianceGuild?.Id;
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
