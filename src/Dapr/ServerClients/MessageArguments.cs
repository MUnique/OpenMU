// <copyright file="MessageArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using MUnique.OpenMU.Interfaces;

/// <summary>
/// Arguments for a game message.
/// </summary>
/// <param name="ServerId">The identifier of the game server which should send the message to its players.
/// The message is published to all game servers, and the others ignore it.</param>
/// <param name="Message">The message.</param>
/// <param name="Type">The type of the message.</param>
public record MessageArguments(int ServerId, string Message, MessageType Type);