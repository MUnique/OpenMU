// <copyright file="WarpInfo.Generated.cs" company="MUnique">
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
/// The Entity Framework Core implementation of <see cref="MUnique.OpenMU.DataModel.Configuration.WarpInfo"/>.
/// </summary>
[Table(nameof(WarpInfo), Schema = SchemaNames.Configuration)]
internal partial class WarpInfo : MUnique.OpenMU.DataModel.Configuration.WarpInfo, IIdentifiable
{
    
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of <see cref="Gate"/>.
    /// </summary>
    public Guid? GateId { get; set; }

    /// <summary>
    /// Gets the raw object of <see cref="Gate" />.
    /// </summary>
    [ForeignKey(nameof(GateId))]
    public ExitGate RawGate
    {
        get => base.Gate as ExitGate;
        set => base.Gate = value;
    }

    /// <inheritdoc/>
    [NotMapped]
    public override MUnique.OpenMU.DataModel.Configuration.ExitGate Gate
    {
        get => base.Gate;set
        {
            base.Gate = value;
            this.GateId = this.RawGate?.Id;
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
