// <copyright file="DiscordRoleLayout.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// A role of the <see cref="DiscordServerLayout"/>.
/// </summary>
public sealed class DiscordRoleLayout
{
    /// <summary>
    /// Gets or sets the key, by which the categories refer to the role.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name. An existing role with this name is adopted.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the color as hexadecimal RGB value, e.g. <c>#E67E22</c>.
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the members of the role are shown separately in the member list.
    /// </summary>
    public bool IsHoisted { get; set; }
}
