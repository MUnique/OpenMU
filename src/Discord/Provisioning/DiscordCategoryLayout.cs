// <copyright file="DiscordCategoryLayout.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Provisioning;

/// <summary>
/// A category of channels of the <see cref="DiscordServerLayout"/>.
/// </summary>
public sealed class DiscordCategoryLayout
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name. An existing category with this name is adopted.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the keys of the roles which can see the category.
    /// If it's empty, everybody can see it.
    /// </summary>
    public List<string> VisibleTo { get; set; } = new();

    /// <summary>
    /// Gets or sets the channels.
    /// </summary>
    public List<DiscordChannelLayout> Channels { get; set; } = new();
}
