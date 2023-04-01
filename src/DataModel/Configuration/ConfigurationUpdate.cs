// <copyright file="ConfigurationUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Describes an applied configuration update.
/// Based on this information, the program can decide which updates are need to
/// be installed next.
/// After a fresh database initialization, an entry exists, so that the maximum
/// version can be determined in this case, too.
/// </summary>
public class ConfigurationUpdate
{
    /// <summary>
    /// Gets or sets the version of the update.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the name of the update.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the update with further information.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the installation timestamp. If it's <c>null</c>, the update wasn't installed yet.
    /// </summary>
    public DateTime? InstalledAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"v{this.Version}: {this.Name}";
    }
}