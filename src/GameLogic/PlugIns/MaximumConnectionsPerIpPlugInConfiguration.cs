// <copyright file="MaximumConnectionsPerIpPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="MaximumConnectionsPerIpPlugIn"/>.
/// </summary>
public class MaximumConnectionsPerIpPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the maximum number of concurrent connections per IP address.
    /// </summary>
    public int MaximumConnectionsPerIp { get; set; } = 3;
}
