// <copyright file="AccountNotificationTypes.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// The types of notifications which a player receives in an external service, e.g. as direct message in Discord.
/// </summary>
[Flags]
public enum AccountNotificationTypes
{
    /// <summary>
    /// No notifications.
    /// </summary>
    None = 0,

    /// <summary>
    /// Somebody tried to log into the account while it was logged in.
    /// </summary>
    LoginAttempt = 1,

    /// <summary>
    /// A character of the account received a letter.
    /// </summary>
    LetterReceived = 2,

    /// <summary>
    /// A friend of a character of the account entered the game.
    /// </summary>
    FriendOnline = 4,
}
