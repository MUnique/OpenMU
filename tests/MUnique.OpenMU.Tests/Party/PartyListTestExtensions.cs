// <copyright file="PartyListTestExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// Helpers for party tests: <see cref="Party.PartyList"/> is backed by an array (<see cref="IReadOnlyList{IPartyMember}"/>), not <see cref="List{IPartyMember}"/>.
/// </summary>
internal static class PartyListTestExtensions
{
    /// <summary>
    /// Gets the index of the member in the party list.
    /// </summary>
    /// <param name="partyList">The current party members.</param>
    /// <param name="member">The member to locate.</param>
    /// <returns>The index as <see cref="byte"/> for kick packet APIs.</returns>
    public static byte IndexOfMember(this IReadOnlyList<IPartyMember> partyList, IPartyMember member)
    {
        for (var i = 0; i < partyList.Count; i++)
        {
            if (ReferenceEquals(partyList[i], member))
            {
                return (byte)i;
            }
        }

        throw new InvalidOperationException($"{nameof(member)} is not in {nameof(partyList)}.");
    }
}
