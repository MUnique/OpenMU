// <copyright file="GuildMasterInactivityConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// The configuration of the <see cref="GuildMasterInactivityPlugIn"/>.
/// </summary>
public class GuildMasterInactivityConfiguration
{
    /// <summary>
    /// Gets or sets the number of days after which a guild master which wasn't seen in the game loses the position.
    /// A value of 0 or less disables the check.
    /// </summary>
    public int InactiveDays { get; set; } = 30;

    /// <summary>
    /// Gets or sets the time between two checks.
    /// </summary>
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// Gets or sets the maximum number of guilds which are handled by one check. The remaining ones are handled by the next checks.
    /// </summary>
    public int MaximumGuildsPerCheck { get; set; } = 3;
}
