// <copyright file="OrbRequirements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Calculates the stat requirements of the orbs, crystals, scrolls and parchments of the
/// item groups 12 and 15, which are used to learn skills.
/// </summary>
/// <remarks>
/// The original item files define these requirements as factors, like for equipment.
/// The client calculates the actual requirement from the factor and the drop level of the item
/// (e.g. Orb of Greater Defense: energy factor 100, drop level 13 => 72 energy), and it only sends
/// the request to use the item when the character fulfills it.
/// For the scrolls of group 15 which have a level requirement, the client uses that level
/// instead of the drop level (e.g. Scroll of Gigantic Storm: energy factor 118, level 220 => 1058 energy).
/// The server compares the stored value directly with the attributes of the character,
/// because these items aren't wearable (see <c>ItemExtensions.GetRequirement</c>),
/// so we store the calculated requirement.
/// </remarks>
internal static class OrbRequirements
{
    /// <summary>
    /// Calculates the energy requirement.
    /// </summary>
    /// <param name="factor">The energy requirement factor of the original item file.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <returns>The required total energy, or 0 if there is no requirement.</returns>
    public static int CalculateEnergy(int factor, byte dropLevel) => Calculate(factor, dropLevel, 4);

    /// <summary>
    /// Calculates the energy requirement of a scroll of item group 15.
    /// </summary>
    /// <param name="factor">The energy requirement factor of the original item file.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <param name="levelRequirement">The level requirement of the item, or 0 if there is none.</param>
    /// <returns>The required total energy, or 0 if there is no requirement.</returns>
    public static int CalculateScrollEnergy(int factor, byte dropLevel, int levelRequirement)
    {
        return levelRequirement > 0
            ? Calculate(factor, levelRequirement, 4)
            : CalculateEnergy(factor, dropLevel);
    }

    /// <summary>
    /// Calculates the strength or agility requirement.
    /// </summary>
    /// <param name="factor">The strength or agility requirement factor of the original item file.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <returns>The required total strength or agility, or 0 if there is no requirement.</returns>
    public static int CalculateStrengthOrAgility(int factor, byte dropLevel) => Calculate(factor, dropLevel, 3);

    private static int Calculate(int factor, int level, int multiplier)
    {
        if (factor == 0)
        {
            return 0;
        }

        return 20 + (multiplier * level * factor / 100);
    }
}
