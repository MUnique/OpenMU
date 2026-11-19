// <copyright file="WebsiteLoginStatus.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The result of a login at the website.
/// </summary>
public enum WebsiteLoginStatus
{
    /// <summary>
    /// The login name and password are correct.
    /// </summary>
    Succeeded,

    /// <summary>
    /// The login name or the password is wrong. Both cases look the same, so the answer doesn't tell whether an account exists.
    /// </summary>
    Failed,

    /// <summary>
    /// The account had too many failed logins recently, so the password isn't checked for now.
    /// </summary>
    LockedOut,
}
