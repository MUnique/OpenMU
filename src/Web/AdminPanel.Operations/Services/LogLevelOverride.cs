// <copyright file="LogLevelOverride.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// A minimum log level which applies to the loggers of a source, instead of the default level.
/// </summary>
public class LogLevelOverride
{
    /// <summary>
    /// Gets or sets the source, which is the name of a logger (usually the full name of a class)
    /// or a prefix of it, like a namespace.
    /// </summary>
    public string Source { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum level of the log entries of the <see cref="Source"/>.
    /// </summary>
    public string Level { get; set; } = LoggingConfigurationService.DefaultLevel;
}
