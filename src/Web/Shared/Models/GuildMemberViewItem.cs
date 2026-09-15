// <copyright file="GuildMemberViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Models;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Data transfer object for a guild member in the admin panel guild detail page.
/// </summary>
public class GuildMemberViewItem
{
    /// <summary>
    /// Gets or sets the identifier of the character. It is the same as the guild member identifier.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Gets or sets the name of the character.
    /// </summary>
    public string CharacterName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the login name of the account the character belongs to, if known.
    /// </summary>
    public string? AccountLoginName { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the account the character belongs to, if known.
    /// It is required to link to the character edit page.
    /// </summary>
    public Guid? AccountId { get; set; }

    /// <summary>
    /// Gets or sets the name of the character class, if known.
    /// </summary>
    public string? CharacterClass { get; set; }

    /// <summary>
    /// Gets or sets the level of the character.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the master level of the character.
    /// </summary>
    public int MasterLevel { get; set; }

    /// <summary>
    /// Gets or sets the position of the member in the guild.
    /// </summary>
    public GuildPosition Position { get; set; }
}
