// <copyright file="ResetFruitConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

/// <summary>
/// The configuration of a <see cref="ResetFruitConsumeHandlerPlugIn"/>.
/// </summary>
public class ResetFruitConsumeHandlerConfiguration
{
    /// <summary>
    /// The default number of points which one durability of a reset fruit takes back.
    /// The game client calculates the same, when it asks the player to confirm the usage of a fruit
    /// which takes back more points than are invested.
    /// </summary>
    internal const int DefaultPointsPerDurability = 10;

    /// <summary>
    /// Gets or sets the number of points which one durability of a reset fruit takes back.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ResetFruitConsumeHandlerConfiguration_PointsPerDurability_Name), Description = nameof(PlugInResources.ResetFruitConsumeHandlerConfiguration_PointsPerDurability_Description))]
    public int PointsPerDurability { get; set; } = DefaultPointsPerDurability;
}
