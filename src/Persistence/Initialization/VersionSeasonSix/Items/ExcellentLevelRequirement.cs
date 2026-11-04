// <copyright file="ExcellentLevelRequirement.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The Season 6 items whose excellent version needs the same level as the normal one: their
/// <see cref="ItemDefinition.ExcellentRaisesLevel"/> is <c>false</c>.
/// </summary>
/// <remarks>
/// The original game server doesn't raise the level requirement of an excellent weapon or armor piece (groups 0 to 11),
/// and it handles the Horn of Fenrir as not excellent. The client leaves the raise out for the items listed here (and for
/// the lucky items, see <see cref="LuckyItems"/>), so it shows the level which the server requires.
/// </remarks>
internal static class ExcellentLevelRequirement
{
    /// <summary>
    /// Gets the items, by group and number, whose excellent version needs the same level as the normal one.
    /// </summary>
    internal static IReadOnlyCollection<(byte Group, short Number)> ItemsWithoutRaise { get; } = new HashSet<(byte Group, short Number)>
    {
        (0, 22), // Bone Blade
        (0, 23), // Explosion Blade
        (2, 14), // Soleil Scepter
        (4, 21), // Sylph Wind Bow
        (5, 12), // Grand Viper Staff
        (5, 19), // Storm Blitz Stick
        (7, 29), // Dragon Knight Helm
        (7, 30), // Venom Mist Helm
        (7, 31), // Sylphid Ray Helm
        (7, 33), // Sunlight Mask
        (7, 43), // Storm Blitz Helm
        (8, 29), // Dragon Knight Armor
        (8, 30), // Venom Mist Armor
        (8, 31), // Sylphid Ray Armor
        (8, 32), // Volcano Armor
        (8, 33), // Sunlight Armor
        (8, 43), // Storm Blitz Armor
        (9, 29), // Dragon Knight Pants
        (9, 30), // Venom Mist Pants
        (9, 31), // Sylphid Ray Pants
        (9, 32), // Volcano Pants
        (9, 33), // Sunlight Pants
        (9, 43), // Storm Blitz Pants
        (10, 29), // Dragon Knight Gloves
        (10, 30), // Venom Mist Gloves
        (10, 31), // Sylphid Ray Gloves
        (10, 32), // Volcano Gloves
        (10, 33), // Sunlight Gloves
        (10, 43), // Storm Blitz Gloves
        (11, 29), // Dragon Knight Boots
        (11, 30), // Venom Mist Boots
        (11, 31), // Sylphid Ray Boots
        (11, 32), // Volcano Boots
        (11, 33), // Sunlight Boots
        (11, 43), // Storm Blitz Boots
        (13, 37), // Horn of Fenrir
    };

    /// <summary>
    /// Sets <see cref="ItemDefinition.ExcellentRaisesLevel"/> to <c>false</c> for the <see cref="ItemsWithoutRaise"/>.
    /// </summary>
    /// <param name="gameConfiguration">The game configuration.</param>
    internal static void Apply(GameConfiguration gameConfiguration)
    {
        foreach (var item in gameConfiguration.Items.Where(item => ItemsWithoutRaise.Contains((item.Group, item.Number))))
        {
            item.ExcellentRaisesLevel = false;
        }
    }
}
