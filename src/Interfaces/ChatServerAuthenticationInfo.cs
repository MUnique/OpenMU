// <copyright file="ChatServerAuthenticationInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

/// <summary>
/// Authentication info of a chat server client.
/// This is created by the chatserver through <see cref="IChatServer.RegisterClient"/>
/// and the chat client needs to provide all of this information to authenticate itself.
/// </summary>
public class ChatServerAuthenticationInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServerAuthenticationInfo" /> class.
    /// </summary>
    /// <param name="index">The index of the client in the room.</param>
    /// <param name="roomId">The room identifier.</param>
    /// <param name="clientName">Name of the client.</param>
    /// <param name="hostAddress">The address of the chat server which hosts the chat room.</param>
    /// <param name="authenticationToken">The authentication token.</param>
    public ChatServerAuthenticationInfo(byte index, ushort roomId, string clientName, string hostAddress, string authenticationToken)
    {
        this.Index = index;
        this.RoomId = roomId;
        this.ClientName = clientName;
        this.AuthenticationToken = authenticationToken;
        this.HostAddress = hostAddress;
        this.AuthenticationRequiredUntil = DateTime.Now.AddSeconds(30);
    }

    /// <summary>
    /// Gets the index of the client in the room.
    /// </summary>
    public byte Index { get; }

    /// <summary>
    /// Gets the room identifier of the room which is reserved for the client.
    /// </summary>
    public ushort RoomId { get; }

    /// <summary>
    /// Gets the name of the client, usually character name.
    /// </summary>
    public string ClientName { get; }

    /// <summary>
    /// Gets the authentication token. It's like a random passwort which the client has to provide to enter the chat room.
    /// </summary>
    public string AuthenticationToken { get; }

    /// <summary>
    /// Gets the (IP-)address of the chat server which hosts the chat room.
    /// </summary>
    public string HostAddress { get; }

    /// <summary>
    /// Gets the datetime until a authentication of at least two clients is required. If this time passed by, the chat room will be closed automatically.
    /// </summary>
    public DateTime AuthenticationRequiredUntil { get; }
}