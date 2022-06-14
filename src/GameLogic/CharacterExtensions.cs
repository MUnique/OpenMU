// <copyright file="CharacterExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions for <see cref="Character"/>.
/// </summary>
public static class CharacterExtensions
{
    private const int MaxLevel = 400;
    private static readonly ushort[] FruitPointsPerLevel = GetFruitPoints(400).ToArray();
    private static readonly ushort[] FruitPointsPerLevelMagicGladiator = GetFruitPoints(700).ToArray();
    private static readonly ushort[] FruitPointsPerLevelDarkLord = GetFruitPoints(500).ToArray();

    /// <summary>
    /// Gets the maximum fruit points of the character.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns>The maximum fruit points of the character.</returns>
    public static ushort GetMaximumFruitPoints(this Character character)
    {
        var index = (int)character.Attributes.First(a => a.Definition == Stats.Level).Value - 1;
        return character.CharacterClass?.FruitCalculation switch
        {
            FruitCalculationStrategy.DarkLord => FruitPointsPerLevelDarkLord[index],
            FruitCalculationStrategy.MagicGladiator => FruitPointsPerLevelMagicGladiator[index],
            _ => FruitPointsPerLevel[index]
        };
    }

    /// <summary>
    /// Determines whether the character has a full ancient set equipped.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns>
    ///   <c>true</c> if the character has a full ancient set equipped; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasFullAncientSetEquipped(this Character character)
    {
        if (character?.Inventory is null)
        {
            return false;
        }

        var equippedAncientSetItems = character.Inventory.Items.Where(i =>
                i.ItemSlot <= InventoryConstants.LastEquippableItemSlotIndex
                && i.ItemSlot >= InventoryConstants.FirstEquippableItemSlotIndex
                && i.ItemSetGroups.Any(group => group.AncientSetDiscriminator > 0))
            .Select(i => new { Item = i.Definition, Set = i.ItemSetGroups.First(s => s.AncientSetDiscriminator > 0) });
        var ancientSets = equippedAncientSetItems.Select(i => i.Set).Distinct();
        return ancientSets.Any(set => set.Items.All(setItem => equippedAncientSetItems.Any(i => i.Item == setItem.ItemDefinition && i.Set == set)));
    }

    /// <summary>
    /// Determines whether the character is able to increase StatAttributes.
    /// </summary>
    /// <param name="character">The character.</param>
    /// <returns>
    ///   <c>true</c> if the character can increase StatAttributes; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanIncreaseStats(this Character character)
    {
        return character.CharacterStatus == CharacterStatus.GameMaster || character.LevelUpPoints > 0;
    }

    private static IEnumerable<ushort> GetFruitPoints(int divisor)
    {
        var current = 2;
        for (int i = 0; i < MaxLevel; i++)
        {
            if (((i + 1) % 10) == 0)
            {
                current += (3 * (i + 11) / divisor) + 2;
            }

            yield return (ushort)current;
        }
    }
}