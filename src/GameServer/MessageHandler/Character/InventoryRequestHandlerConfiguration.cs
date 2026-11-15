// <copyright file="InventoryRequestHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.MessageHandler.Character;

/// <summary>
/// Limits the frequency of client-requested full inventory serialization.
/// </summary>
public class InventoryRequestHandlerConfiguration
{
    /// <summary>
    /// Gets or sets the minimum interval between snapshot starts for each player.
    /// Requests during this interval await a shared fresh snapshot instead of being dropped.
    /// </summary>
    public TimeSpan MinimumInterval { get; set; } = TimeSpan.FromMilliseconds(250);
}
