// <copyright file="CastleSiegeParticipant.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Tracks a character's participation in the active Castle Siege battle.
/// </summary>
public class CastleSiegeParticipant
{
    /// <summary>
    /// Gets or sets the persistent character identifier.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the runtime guild identifier.
    /// </summary>
    public uint GuildId { get; set; }

    /// <summary>
    /// Gets or sets the number of seconds for which the character participated.
    /// </summary>
    public int Seconds { get; set; }
}
