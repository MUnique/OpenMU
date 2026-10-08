// <copyright file="SiegePotionConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Configuration for the <see cref="SiegePotionConsumeHandlerPlugIn"/>.
/// </summary>
/// <remarks>
/// Both potions share one item definition and differ only by item level,
/// so a single <see cref="ItemDefinition.ConsumeEffect"/> can't express them.
/// </remarks>
public class SiegePotionConsumeHandlerConfiguration
{
    /// <summary>
    /// The default number of the magic effect of the Potion of Bless.
    /// </summary>
    internal const short DefaultBlessEffectNumber = 10;

    /// <summary>
    /// The default number of the magic effect of the Potion of Soul.
    /// </summary>
    internal const short DefaultSoulEffectNumber = 11;

    /// <summary>
    /// Gets or sets the number of the magic effect which is applied by the Potion of Bless (item level 0).
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.SiegePotionConsumeHandlerConfiguration_BlessEffectNumber_Name))]
    public short BlessEffectNumber { get; set; } = DefaultBlessEffectNumber;

    /// <summary>
    /// Gets or sets the number of the magic effect which is applied by the Potion of Soul (item level 1).
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.SiegePotionConsumeHandlerConfiguration_SoulEffectNumber_Name))]
    public short SoulEffectNumber { get; set; } = DefaultSoulEffectNumber;
}
