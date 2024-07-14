// -----------------------------------------------------------------------
// <copyright file="SoulJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Jewel of Soul which increases the item level by one until the level of 9 with a chance of 50%.
/// </summary>
[Guid("A76CDA49-1C56-401A-96D1-294D9A68A7B9")]
[PlugIn(nameof(SoulJewelConsumeHandlerPlugIn), "Plugin which handles the jewel of soul consumption.")]
public class SoulJewelConsumeHandlerPlugIn : UpgradeItemLevelJewelConsumeHandlerPlugIn<UpgradeItemLevelConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SoulJewelConsumeHandlerPlugIn"/> class.
    /// </summary>
    public SoulJewelConsumeHandlerPlugIn()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoulJewelConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="randomizer">The randomizer.</param>
    internal SoulJewelConsumeHandlerPlugIn(IRandomizer randomizer)
        :base(randomizer)
    {
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfSoul;

    /// <inheritdoc />
    public override object CreateDefaultConfig() => new UpgradeItemLevelConfiguration
    {
        MaximumLevel = 8,
        MinimumLevel = 0,
        SuccessRatePercentage = 50,
        SuccessRateBonusWithLuckPercentage = 25,
        ResetToLevel0WhenFailMinLevel = 7,
    };
}