// <copyright file="CentralServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// The addresses of the central server, which hosts the login, guild, friend, chat and connect server.
/// </summary>
public static class CentralServer
{
    /// <summary>
    /// The dapr app id of the central server.
    /// </summary>
    public const string AppId = "centralServer";

    /// <summary>
    /// The route prefix of the methods of the login server.
    /// </summary>
    public const string LoginServerRoute = "login";

    /// <summary>
    /// The route prefix of the methods of the guild server.
    /// </summary>
    public const string GuildServerRoute = "guild";

    /// <summary>
    /// The route prefix of the methods of the friend server.
    /// </summary>
    public const string FriendServerRoute = "friend";
}
