// <copyright file="CastleSiegeNpcDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a castle siege NPC instance, including its spawn location, side, and persistence settings.
/// </summary>
[Cloneable]
public partial class CastleSiegeNpcDefinition
{
    /// <summary>
    /// Gets or sets the monster definition template for this NPC.
    /// </summary>
    public virtual MonsterDefinition? MonsterDefinition { get; set; }

    /// <summary>
    /// Gets or sets the unique instance identifier within its NPC type.
    /// </summary>
    public byte InstanceId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this NPC's state is persisted to the database between sieges.
    /// </summary>
    public bool IsPersistedToDatabase { get; set; }

    /// <summary>
    /// Gets or sets the default join side this NPC belongs to.
    /// </summary>
    public CastleSiegeJoinSide DefaultSide { get; set; }

    /// <summary>
    /// Gets or sets the X coordinate of the NPC's spawn position.
    /// </summary>
    public short SpawnX { get; set; }

    /// <summary>
    /// Gets or sets the Y coordinate of the NPC's spawn position.
    /// </summary>
    public short SpawnY { get; set; }

    /// <summary>
    /// Gets or sets the facing direction of the NPC at spawn.
    /// </summary>
    public Direction Direction { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.MonsterDefinition} #{this.InstanceId} at ({this.SpawnX},{this.SpawnY})";
    }
}
