// <copyright file="WorldChatMessageArguments.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

/// <summary>
/// Arguments for a message of the world chat.
/// </summary>
/// <param name="Sender">The sender.</param>
/// <param name="Message">The message.</param>
public record WorldChatMessageArguments(string Sender, string Message);
