// <copyright file="KalimaTicketConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// Configuration for the <see cref="KalimaTicketConsumeHandlerPlugIn"/>.
/// </summary>
/// <remarks>
/// The default values are the minimum character levels which the game client shows for the levels of Kalima.
/// </remarks>
public class KalimaTicketConsumeHandlerConfiguration
{
    /// <summary>
    /// Gets or sets the minimum character levels of the levels of Kalima, starting at the first level.
    /// The ticket opens the gate to the highest level of Kalima whose minimum character level the character reached.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.KalimaTicketConsumeHandlerConfiguration_MinimumLevels_Name))]
    public IList<int> MinimumLevels { get; set; } = new List<int> { 40, 131, 181, 231, 281, 331, 350 };

    /// <summary>
    /// Gets or sets the minimum character levels of the levels of Kalima for special characters (Magic Gladiator, Dark Lord and Rage Fighter),
    /// starting at the first level.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.KalimaTicketConsumeHandlerConfiguration_SpecialCharacterMinimumLevels_Name))]
    public IList<int> SpecialCharacterMinimumLevels { get; set; } = new List<int> { 20, 111, 161, 211, 261, 311, 350 };
}
