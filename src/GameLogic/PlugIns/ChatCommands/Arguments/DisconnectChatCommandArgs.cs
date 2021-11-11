// <copyright file="DisconnectChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by DisconnectChatCommandPlugIn.
/// </summary>
public class DisconnectChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    [Argument("char")]
    public string? CharacterName { get; set; }
}