// <copyright file="LoggingSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// The logging settings, as they are stored in the configuration file.
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// Gets or sets the minimum level of the log entries of all sources without an override.
    /// </summary>
    public string DefaultLevel { get; set; } = LoggingConfigurationService.DefaultLevel;

    /// <summary>
    /// Gets the minimum levels of specific sources.
    /// </summary>
    public List<LogLevelOverride> Overrides { get; } = new();

    /// <summary>
    /// Gets or sets the whole logging section of the configuration file, as indented JSON.
    /// </summary>
    public string SectionJson { get; set; } = "{}";
}
