// -----------------------------------------------------------------------
// <copyright file="SoulJewelConsumeHandlerPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Consume handler for the Jewel of Soul which increases the item level by one until the level of 9 with a chance of 50%.
/// </summary>
[Guid("A76CDA49-1C56-401A-96D1-294D9A68A7B9")]
[PlugIn(nameof(SoulJewelConsumeHandlerPlugIn), "Plugin which handles the jewel of soul consumption.")]
public class SoulJewelConsumeHandlerPlugIn : ItemModifyConsumeHandlerPlugIn
{
    private readonly IRandomizer _randomizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoulJewelConsumeHandlerPlugIn"/> class.
    /// </summary>
    public SoulJewelConsumeHandlerPlugIn()
        : this(Rand.GetRandomizer())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoulJewelConsumeHandlerPlugIn"/> class.
    /// </summary>
    /// <param name="randomizer">The randomizer.</param>
    public SoulJewelConsumeHandlerPlugIn(IRandomizer randomizer)
    {
        this._randomizer = randomizer;
    }

    /// <inheritdoc />
    public override ItemIdentifier Key => ItemConstants.JewelOfSoul;

    /// <inheritdoc/>
    protected override bool ModifyItem(Item item, IContext persistenceContext)
    {
        if (!item.CanLevelBeUpgraded())
        {
            return false;
        }

        if (item.Level > 8)
        {
            return false;
        }

        int percent = 50;
        if (ItemHasLuck(item))
        {
            percent += 25;
        }

        if (this._randomizer.NextRandomBool(percent))
        {
            item.Level++;
            return true; // true doesn't mean that it was successful, just that the consumption happened.
        }

        if (item.Level > 6)
        {
            item.Level = 0;
        }
        else
        {
            item.Level = (byte)Math.Max(item.Level - 1, 0);
        }

        return true;
    }

    private static bool ItemHasLuck(Item item)
    {
        return item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck);
    }
}