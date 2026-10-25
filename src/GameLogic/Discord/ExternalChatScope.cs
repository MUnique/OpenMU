// <copyright file="ExternalChatScope.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Discord;

/// <summary>
/// The chat of the game which a message written outside of the game belongs to.
/// </summary>
public enum ExternalChatScope
{
    /// <summary>
    /// The chat of a guild.
    /// </summary>
    Guild,

    /// <summary>
    /// The chat of an alliance.
    /// </summary>
    Alliance,

    /// <summary>
    /// The world chat.
    /// </summary>
    World,
}
