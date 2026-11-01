// <copyright file="PartyColorHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Components;

/// <summary>
/// Assigns a stable badge color per party master, so all members of one party share
/// the same color and different parties are visually distinct.
/// Uses FNV-1a instead of the process-randomized string hash function, so colors
/// don't change on every restart of the admin panel.
/// </summary>
public static class PartyColorHelper
{
    /// <summary>
    /// Gets the badge background color for the given party master.
    /// </summary>
    /// <param name="partyMaster">The character name of the party master.</param>
    public static string GetPartyColor(string partyMaster)
    {
        return $"hsl({Fnv1aUpperInvariant(partyMaster) % 360}, 45%, 45%)";
    }

    /// <summary>
    /// Computes the 32-bit FNV-1a hash of the value, folding casing without allocating.
    /// </summary>
    /// <param name="value">The value to hash.</param>
    internal static uint Fnv1aUpperInvariant(string value)
    {
        const uint offsetBasis = 2166136261;
        const uint prime = 16777619;

        var hash = offsetBasis;
        foreach (var c in value)
        {
            hash ^= char.ToUpperInvariant(c);
            hash *= prime;
        }

        return hash;
    }
}
