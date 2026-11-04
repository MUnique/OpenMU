// <copyright file="AdminLoginStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Auth;

/// <summary>
/// The result status of a login attempt.
/// </summary>
public enum AdminLoginStatus
{
    /// <summary>
    /// The credentials were wrong or the user is not allowed to log in.
    /// </summary>
    Failed,

    /// <summary>
    /// The user is locked out because of too many failed attempts.
    /// </summary>
    LockedOut,

    /// <summary>
    /// The password was correct, but a second factor is required now.
    /// </summary>
    TwoFactorRequired,

    /// <summary>
    /// The login succeeded.
    /// </summary>
    Succeeded,
}
