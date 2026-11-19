// <copyright file="WebsiteLoginResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

/// <summary>
/// The result of a login at the website.
/// </summary>
/// <param name="Status">The status of the login.</param>
/// <param name="Account">The account, if the login succeeded.</param>
public sealed record WebsiteLoginResult(WebsiteLoginStatus Status, WebsiteAccount? Account);
