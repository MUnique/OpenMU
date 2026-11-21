// <copyright file="ItemExpirationConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// The configuration of the <see cref="ItemExpirationPlugIn"/>.
/// </summary>
public class ItemExpirationConfiguration
{
    /// <summary>
    /// Gets or sets the time between two checks for expired items. An item expires on time, but the player
    /// is told and the client is updated at the next check.
    /// </summary>
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromMinutes(1);
}
