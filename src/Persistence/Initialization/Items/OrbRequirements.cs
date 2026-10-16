// <copyright file="OrbRequirements.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Calculates the stat requirements of the orbs, crystals and scrolls of item group 12,
/// which are used to learn skills.
/// </summary>
/// <remarks>
/// The original item files define these requirements as factors, like for equipment.
/// The client calculates the actual requirement from the factor and the drop level of the item
/// (e.g. Orb of Greater Defense: energy factor 100, drop level 13 => 72 energy), and it only sends
/// the request to use the item when the character fulfills it.
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
    /// Calculates the strength or agility requirement.
    /// </summary>
    /// <param name="factor">The strength or agility requirement factor of the original item file.</param>
    /// <param name="dropLevel">The drop level of the item.</param>
    /// <returns>The required total strength or agility, or 0 if there is no requirement.</returns>
    public static int CalculateStrengthOrAgility(int factor, byte dropLevel) => Calculate(factor, dropLevel, 3);

    private static int Calculate(int factor, byte dropLevel, int multiplier)
    {
        if (factor == 0)
        {
            return 0;
        }

        return 20 + (multiplier * dropLevel * factor / 100);
    }
}
