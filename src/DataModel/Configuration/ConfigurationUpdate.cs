// <copyright file="ConfigurationUpdate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Describes an applied configuration update.
/// Based on this information, the program can decide which updates are need to
/// be installed next.
/// After a fresh database initialization, entries exist for all known updates,
/// so that nothing is applied twice.
/// </summary>
public class ConfigurationUpdate
{
    /// <summary>
    /// Gets or sets the key of the update. This is the <see cref="Type.GUID"/> of the update plug-in implementation.
    /// </summary>
    public Guid Key { get; set; }

    /// <summary>
    /// Gets or sets the version of the update plug-in which was installed.
    /// If the plug-in code has a higher version, the update is offered again.
    /// </summary>
    public int Version { get; set; } = 1;

    /// <summary>
    /// Gets or sets the name of the update.
    /// </summary>
    public LocalizedString Name { get; set; }

    /// <summary>
    /// Gets or sets the description of the update with further information.
    /// </summary>
    public LocalizedString Description { get; set; }

    /// <summary>
    /// Gets or sets the release date.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date of the last change of the update plug-in.
    /// If it's <c>null</c>, the update was never changed since its creation.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the installation timestamp. If it's <c>null</c>, the update wasn't installed yet.
    /// </summary>
    public DateTime? InstalledAt { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Name} ({this.Key})";
    }
}