// <copyright file="LevelBonus.Generated.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

//------------------------------------------------------------------------------
// <auto-generated>
//     This source code was auto-generated by a roslyn code generator.
// </auto-generated>
//------------------------------------------------------------------------------

// ReSharper disable All

namespace MUnique.OpenMU.Persistence.BasicModel;

using MUnique.OpenMU.Persistence.Json;

/// <summary>
/// A plain implementation of <see cref="LevelBonus"/>.
/// </summary>
public partial class LevelBonus : MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus, IIdentifiable, IConvertibleTo<LevelBonus>
{
    /// <inheritdoc />
    public LevelBonus()
    {
    }

    /// <inheritdoc />
    public LevelBonus(System.Int32 level, System.Single constantValue)
        : base(level, constantValue)
    {
    }

    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new LevelBonus();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.LevelBonus other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        base.AssignValuesOf(other, gameConfiguration);
        this.Id = other.GetId();
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
    public LevelBonus Convert() => this;
}
