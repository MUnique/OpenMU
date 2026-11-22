// <copyright file="ExcellentLevelRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Decides which Season 6 items need the same level for their excellent version as for the normal one: their
/// <see cref="ItemDefinition.ExcellentRaisesLevel"/> is <c>false</c>.
/// </summary>
/// <remarks>
/// The original game servers (1.00.90, 1.00.93 and the common Season 6 emulator) never raise the level requirement of
/// an excellent weapon, shield or armor piece (groups 0 to 11), and they handle the Horn of Fenrir as not excellent. Only
/// these items and the wings and other items of the later groups have a level requirement which the raise could
/// change; most weapons and armor pieces have none at all.
/// The client of version 1.04d leaves the raise out only for a part of these items: the 380 items up to the Aura (Storm
/// Blitz) set, the lucky items and the Horn of Fenrir. It still raises it for the Phoenix Soul set of the Rage Fighter
/// and the socket weapons of level 380, so it shows 400 for them. Here, all of them need 380, as on the server.
/// </remarks>
internal static class ExcellentLevelRequirement
{
    private const byte LastArmorGroup = (byte)ItemGroups.Boots;

    /// <summary>
    /// Determines whether the excellent version of the item needs a higher level than the normal one.
    /// </summary>
    /// <param name="item">The item definition.</param>
    /// <returns><c>false</c> for the weapons, shields and armor pieces with a level requirement and the Horn of Fenrir.</returns>
    internal static bool RaisesLevel(ItemDefinition item)
    {
        var isHornOfFenrir = item.Group == (byte)ItemGroups.Misc1 && item.Number == 37;
        var isEquipmentWithLevel = item.Group <= LastArmorGroup
                                   && item.Requirements.Any(requirement => requirement.Attribute == Stats.Level && requirement.MinimumValue > 0);
        return !isHornOfFenrir && !isEquipmentWithLevel;
    }

    /// <summary>
    /// Sets <see cref="ItemDefinition.ExcellentRaisesLevel"/> to <c>false</c> for the items for which
    /// <see cref="RaisesLevel"/> is <c>false</c>. The other items are left alone.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <returns>The items which were changed.</returns>
    internal static IReadOnlyList<ItemDefinition> Apply(GameConfiguration gameConfiguration)
    {
        var changed = gameConfiguration.Items.Where(item => item.ExcellentRaisesLevel && !RaisesLevel(item)).ToList();
        changed.ForEach(item => item.ExcellentRaisesLevel = false);
        return changed;
    }
}
