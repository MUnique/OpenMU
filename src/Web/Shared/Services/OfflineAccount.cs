// <copyright file="OfflineAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Keeps the displayed data of an active offline session.
/// </summary>
public record OfflineAccount(string LoginName, byte ServerId, DateTime StartedAt);
