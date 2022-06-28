// <copyright file="ChatRoomCreationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a chat room creation.
/// </summary>
/// <param name="AuthenticationInfo">Authentication information of the player who should get notified about the created chat room.</param>
/// <param name="FriendName">Name of the friend player which is expected to be in the chat room.</param>
public record ChatRoomCreationArguments(
    ChatServerAuthenticationInfo AuthenticationInfo,
    string FriendName);