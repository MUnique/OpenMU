// <copyright file="OnlineAccountOrdering.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Shared ordering for the online-accounts tables: partied players first so each party's
/// members stay contiguous, then by party master, account and character.
/// </summary>
public static class OnlineAccountOrdering
{
    /// <summary>
    /// Orders the accounts so party members stay contiguous.
    /// </summary>
    /// <typeparam name="T">The account type.</typeparam>
    /// <param name="accounts">The accounts to order.</param>
    /// <returns>The ordered accounts.</returns>
    public static IOrderedEnumerable<T> OrderPartyGrouped<T>(this IEnumerable<T> accounts)
        where T : IPartyGroupedAccount
    {
        return accounts
            .OrderByDescending(a => a.PartyMaster is not null)
            .ThenBy(a => a.PartyMaster)
            .ThenBy(a => a.LoginName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(a => a.CharacterName, StringComparer.OrdinalIgnoreCase);
    }
}
