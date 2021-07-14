// <copyright file="IChatServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    /// <summary>
    /// The interface for a chat server.
    /// </summary>
    public interface IChatServer : IManageableServer
    {
        /// <summary>
        /// Gets the ip address.
        /// </summary>
        /// <returns>The ip address.</returns>
        string IpAddress { get; }

        /// <summary>
        /// Registers the client to the server.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="clientName">Name of the client.</param>
        /// <returns>The success.</returns>
        ChatServerAuthenticationInfo RegisterClient(ushort roomId, string clientName);

        /// <summary>
        /// Creates the chat room.
        /// </summary>
        /// <returns>The new chat room id.</returns>
        ushort CreateChatRoom();
    }
}
