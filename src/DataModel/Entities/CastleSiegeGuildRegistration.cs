// <copyright file="CastleSiegeGuildRegistration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Stores a guild's registration data for the current castle siege cycle,
/// including the number of emblems submitted to determine attacking guilds.
/// </summary>
[AggregateRoot]
public class CastleSiegeGuildRegistration
{
    /// <summary>
    /// Gets or sets the unique identifier of this registration record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the persistent identifier of the registered guild.
    /// </summary>
    public Guid GuildId { get; set; }

    /// <summary>
    /// Gets or sets the guild name, denormalized for convenience to avoid extra lookups during siege processing.
    /// </summary>
    public string GuildName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of Emblems of Lord registered by this guild.
    /// </summary>
    public int Marks { get; set; }

    /// <summary>
    /// Gets or sets the insertion order of this registration, used for tie-breaking when guilds have equal marks.
    /// </summary>
    public int RegistrationOrder { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.GuildName} (Marks={this.Marks}, Order={this.RegistrationOrder})";
    }
}
