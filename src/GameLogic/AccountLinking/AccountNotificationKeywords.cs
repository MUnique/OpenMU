// <copyright file="AccountNotificationKeywords.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.AccountLinking;

using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The keywords of the <see cref="AccountNotificationTypes"/>, which are used in commands, e.g. <c>/discord notify letter</c>.
/// </summary>
public static class AccountNotificationKeywords
{
    /// <summary>
    /// The keyword of <see cref="AccountNotificationTypes.LoginAttempt"/>.
    /// </summary>
    public const string LoginAttempt = "login";

    /// <summary>
    /// The keyword of <see cref="AccountNotificationTypes.LetterReceived"/>.
    /// </summary>
    public const string LetterReceived = "letter";

    /// <summary>
    /// The keyword of <see cref="AccountNotificationTypes.FriendOnline"/>.
    /// </summary>
    public const string FriendOnline = "friend";

    /// <summary>
    /// Gets the type of a keyword.
    /// </summary>
    /// <param name="keyword">The keyword.</param>
    /// <returns>The type; or <see cref="AccountNotificationTypes.None"/>, if the keyword is unknown.</returns>
    public static AccountNotificationTypes Parse(string? keyword) => keyword?.ToLowerInvariant() switch
    {
        LoginAttempt => AccountNotificationTypes.LoginAttempt,
        LetterReceived => AccountNotificationTypes.LetterReceived,
        FriendOnline => AccountNotificationTypes.FriendOnline,
        _ => AccountNotificationTypes.None,
    };

    /// <summary>
    /// Formats the types as keywords, e.g. <c>login, letter</c>.
    /// </summary>
    /// <param name="types">The types.</param>
    /// <returns>The keywords; or <c>-</c>, if there are none.</returns>
    public static string Format(AccountNotificationTypes types)
    {
        var keywords = new List<string>();
        if (types.HasFlag(AccountNotificationTypes.LoginAttempt))
        {
            keywords.Add(LoginAttempt);
        }

        if (types.HasFlag(AccountNotificationTypes.LetterReceived))
        {
            keywords.Add(LetterReceived);
        }

        if (types.HasFlag(AccountNotificationTypes.FriendOnline))
        {
            keywords.Add(FriendOnline);
        }

        return keywords.Count > 0 ? string.Join(", ", keywords) : "-";
    }
}
