// <copyright file="VulcanusMapConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration of the plugins of the Gladiator's Honor, which only apply in Vulcanus.
/// </summary>
public class VulcanusMapConfiguration
{
    /// <summary>
    /// The default number of the map Vulcanus.
    /// </summary>
    internal const short DefaultMapNumber = 63;

    /// <summary>
    /// Gets or sets the number of the map in which the Gladiator's Honor can be used and gives its bonus.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.VulcanusMapConfiguration_MapNumber_Name))]
    public short MapNumber { get; set; } = DefaultMapNumber;
}
