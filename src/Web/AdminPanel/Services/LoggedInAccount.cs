// <copyright file="LoggedInAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Services;

/// <summary>
/// Keeps the displayed data of a logged-in account.
/// </summary>
public record LoggedInAccount(string LoginName, byte Server);