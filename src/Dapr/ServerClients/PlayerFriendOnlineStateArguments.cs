// <copyright file="PlayerFriendOnlineStateArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a friend online state change.
/// </summary>
/// <param name="CharacterId">The character identifier.</param>
/// <param name="CharacterName">The character name.</param>
/// <param name="ServerId">The server identifier.</param>
/// <param name="IsVisible">A value indicating whether the character is visible.</param>
public record PlayerFriendOnlineStateArguments(Guid CharacterId, string CharacterName, byte ServerId, bool IsVisible);