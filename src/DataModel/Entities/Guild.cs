// <copyright file="Guild.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// A guild is a group of players who like to play together.
/// </summary>
[AggregateRoot]
public class Guild : OpenMU.Interfaces.Guild
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the members.
    /// </summary>
    public virtual ICollection<GuildMember> Members { get; protected set; } = null!;

    /// <inheritdoc />
    public override string ToString()
    {
        return this.Name ?? "<Guild>";
    }
}