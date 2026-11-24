// <copyright file="PartyExperienceBonus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Determines the experience bonus of a party, depending on its size and composition.
/// </summary>
/// <remarks>
/// <para>
/// The bonuses are configurable through attributes, which are read from the attribute system of the party members,
/// so they can be defined globally, but also per map, account or character.
/// The first party member decides, as it does for the random experience multipliers.
/// </para>
/// <para>
/// The experience of a party with n members is multiplied by <see cref="Stats.PartyExperienceMultiplier"/>^(n - 1),
/// which is 1.05^(n - 1) by default. A party with at least <see cref="MinimumSetPartySize"/> members, in which every
/// member belongs to a different character class line (e.g. Dark Wizard, Dark Knight and Fairy Elf), is a "set party"
/// and additionally gets <see cref="Stats.SetPartyExperienceBonus"/> added, which is 0 by default.
/// </para>
/// </remarks>
internal static class PartyExperienceBonus
{
    /// <summary>
    /// The minimum number of members of a set party.
    /// </summary>
    internal const int MinimumSetPartySize = 3;

    /// <summary>
    /// The minimum number of members of a party which gets an experience bonus.
    /// </summary>
    internal const int MinimumBonusPartySize = 2;

    /// <summary>
    /// Gets the multiplier which is applied to the experience of the party.
    /// </summary>
    /// <param name="members">The party members which share the experience.</param>
    /// <returns>The multiplier, which is <c>1</c> if the party doesn't get a bonus.</returns>
    public static double GetMultiplier(IReadOnlyList<Player> members)
    {
        if (members.Count < MinimumBonusPartySize || members[0].Attributes is not { } attributes)
        {
            return 1;
        }

        var multiplier = 1.0;
        var partyMultiplier = ToDouble(attributes[Stats.PartyExperienceMultiplier]);
        if (partyMultiplier > 0)
        {
            multiplier = Math.Pow(partyMultiplier, members.Count - 1);
        }

        var setPartyBonus = ToDouble(attributes[Stats.SetPartyExperienceBonus]);
        if (setPartyBonus > 0 && IsSetParty(members))
        {
            multiplier += setPartyBonus;
        }

        return multiplier;
    }

    /// <summary>
    /// Determines whether the members form a set party, which means that they are at least
    /// <see cref="MinimumSetPartySize"/> and each of them belongs to a different character class line.
    /// </summary>
    /// <param name="members">The party members.</param>
    /// <returns><c>true</c>, if the members form a set party.</returns>
    public static bool IsSetParty(IReadOnlyList<Player> members)
    {
        if (members.Count < MinimumSetPartySize)
        {
            return false;
        }

        // Resolve the class line only once per member. The class hierarchy is configuration data and
        // can be edited at runtime, so it is intentionally not cached across calls.
        // Reference equality is used on purpose: the members share one game configuration, so their
        // classes are the same instances, while CharacterClass overrides Equals by its id.
        var classLines = new HashSet<CharacterClass>(members.Count, ReferenceEqualityComparer.Instance);
        foreach (var member in members)
        {
            if (GetLastClassOfLine(member) is not { } classLine
                || !classLines.Add(classLine))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Gets the last class of the line of the character class of the player.
    /// </summary>
    private static CharacterClass? GetLastClassOfLine(Player player)
    {
        var characterClass = player.SelectedCharacter?.CharacterClass;
        if (characterClass is null)
        {
            return null;
        }

        // The class count bounds the loop, so a misconfigured cycle can't hang the game logic.
        var maximumSteps = player.GameContext.Configuration.CharacterClasses.Count;
        for (var step = 0; step < maximumSteps && characterClass.NextGenerationClass is { } nextClass; step++)
        {
            characterClass = nextClass;
        }

        return characterClass;
    }

    /// <summary>
    /// Converts the attribute value to a double.
    /// </summary>
    /// <remarks>
    /// The attribute values are floats, which can't represent fractions like 1.05 exactly. A plain conversion would
    /// turn it into 1.0499999..., and the experience would differ slightly from the one calculated with
    /// the literal 1.05. Rounding to seven decimal places, which is more than a float can hold reliably, avoids that.
    /// </remarks>
    private static double ToDouble(float value)
    {
        return Math.Round((double)value, 7);
    }
}
