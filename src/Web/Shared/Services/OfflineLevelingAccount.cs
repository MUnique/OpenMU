// <copyright file="OfflineLevelingAccount.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Keeps the displayed data of an active offline leveling session.
/// </summary>
public record OfflineLevelingAccount(string LoginName, string CharacterName, byte ServerId, DateTime StartedAt);
