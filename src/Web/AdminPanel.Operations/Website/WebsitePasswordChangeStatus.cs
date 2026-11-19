// <copyright file="WebsitePasswordChangeStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The result of a password change at the website.
/// </summary>
public enum WebsitePasswordChangeStatus
{
    /// <summary>
    /// The password has been changed.
    /// </summary>
    Changed,

    /// <summary>
    /// The login name or the current password is wrong.
    /// </summary>
    Failed,

    /// <summary>
    /// The account had too many failed logins recently, so the password isn't checked for now.
    /// </summary>
    LockedOut,

    /// <summary>
    /// The account is online. The game server saves the account when the player leaves, so the password can only be changed while it's offline.
    /// </summary>
    AccountOnline,

    /// <summary>
    /// A character of the account is offline leveling. The account is logged off, but the character stays in the game
    /// and the game server saves the account when the offline leveling ends, so the password can't be changed until then.
    /// </summary>
    OfflineLeveling,

    /// <summary>
    /// A game server couldn't tell whether a character of the account is offline leveling, so the password isn't changed:
    /// the game server would overwrite it, if a character is offline leveling there.
    /// </summary>
    GameServerUnavailable,
}
