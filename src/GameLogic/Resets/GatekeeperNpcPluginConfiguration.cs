// <copyright file="GatekeeperNpcPluginConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Configuration for the <see cref="GatekeeperNpcPlugin"/>.
/// </summary>
public class GatekeeperNpcPluginConfiguration
{
    /// <summary>
    /// The default number of the 'Gatekeeper' NPC in the Barracks of Balgass.
    /// </summary>
    internal const short DefaultGatekeeperNumber = 408;

    /// <summary>
    /// Gets or sets the number of the 'Gatekeeper' NPC in the Barracks of Balgass.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.GatekeeperNpcPluginConfiguration_GatekeeperNumber_Name))]
    public short GatekeeperNumber { get; set; } = DefaultGatekeeperNumber;
}
