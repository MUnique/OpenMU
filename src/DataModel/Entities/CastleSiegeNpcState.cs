// <copyright file="CastleSiegeNpcState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Persistent state of a single castle siege NPC between siege cycles.
/// </summary>
public class CastleSiegeNpcState
{
    /// <summary>
    /// Gets or sets the unique identifier of this NPC state.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the monster definition number that identifies the NPC template.
    /// </summary>
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets the instance identifier matching <see cref="MUnique.OpenMU.DataModel.Configuration.CastleSiegeNpcDefinition.InstanceId"/>.
    /// </summary>
    public byte InstanceId { get; set; }

    /// <summary>
    /// Gets or sets the current defense upgrade level (0–3).
    /// </summary>
    public byte DefenseLevel { get; set; }

    /// <summary>
    /// Gets or sets the current HP regeneration upgrade level (0–3).
    /// </summary>
    public byte RegenLevel { get; set; }

    /// <summary>
    /// Gets or sets the current maximum HP upgrade level (0–3).
    /// </summary>
    public byte LifeLevel { get; set; }

    /// <summary>
    /// Gets or sets the current HP of the NPC. A value of 0 means the NPC is destroyed.
    /// </summary>
    public int CurrentHp { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"NPC {this.MonsterNumber} #{this.InstanceId} (HP={this.CurrentHp})";
    }
}
