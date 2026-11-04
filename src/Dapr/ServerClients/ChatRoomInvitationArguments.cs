// <copyright file="ChatRoomInvitationArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// The arguments for a chat room invitation.
/// </summary>
public record ChatRoomInvitationArguments(
    string CharacterName,
    string FriendName,
    ushort RoomNumber);