// <copyright file="ItemOptionOfLevel.Generated.cs" company="MUnique">
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
/// A plain implementation of <see cref="ItemOptionOfLevel"/>.
/// </summary>
public partial class ItemOptionOfLevel : MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionOfLevel, IIdentifiable, IConvertibleTo<ItemOptionOfLevel>
{
    
    /// <summary>
    /// Gets or sets the identifier of this instance.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Gets the raw object of <see cref="PowerUpDefinition" />.
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("powerUpDefinition")]
    public PowerUpDefinition RawPowerUpDefinition
    {
        get => base.PowerUpDefinition as PowerUpDefinition;
        set => base.PowerUpDefinition = value;
    }

    /// <inheritdoc/>
    [System.Text.Json.Serialization.JsonIgnore]
    public override MUnique.OpenMU.DataModel.Attributes.PowerUpDefinition PowerUpDefinition
    {
        get => base.PowerUpDefinition;
        set => base.PowerUpDefinition = value;
    }

    /// <inheritdoc />
    public override MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionOfLevel Clone(MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
    {
        var clone = new ItemOptionOfLevel();
        clone.AssignValuesOf(this, gameConfiguration);
        return clone;
    }
    
    /// <inheritdoc />
    public override void AssignValuesOf(MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionOfLevel other, MUnique.OpenMU.DataModel.Configuration.GameConfiguration gameConfiguration)
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
    public ItemOptionOfLevel Convert() => this;
}
