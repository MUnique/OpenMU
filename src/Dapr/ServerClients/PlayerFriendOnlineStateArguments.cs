// <copyright file="PlayerFriendOnlineStateArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a friend online state change.
/// </summary>
public record PlayerFriendOnlineStateArguments(Guid CharacterId, string CharacterName, byte ServerId, bool IsVisible);