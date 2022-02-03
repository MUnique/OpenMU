// <copyright file="ChatRoomCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a chat room creation.
/// </summary>
public record ChatRoomCreationArguments(
    ChatServerAuthenticationInfo AuthenticationInfo,
    string FriendName);