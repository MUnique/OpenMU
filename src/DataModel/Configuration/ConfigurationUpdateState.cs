// <copyright file="ConfigurationUpdateState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Describes the current update state of the configuration.
/// </summary>
public class ConfigurationUpdateState
{
    /// <summary>
    /// Gets or sets the initialization key.
    /// </summary>
    public string? InitializationKey { get; set; }

    /// <summary>
    /// Gets or sets the highest <see cref="ConfigurationUpdate.Version"/> which is installed.
    /// </summary>
    public int CurrentInstalledVersion { get; set; }
}