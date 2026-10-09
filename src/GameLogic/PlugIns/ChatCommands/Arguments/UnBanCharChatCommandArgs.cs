// <copyright file="UnBanCharChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by UnBanCharChatCommandPlugIn.
/// </summary>
public class UnBanCharChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    [Argument("char")]
    [ValueReference(ChatCommandValueReference.CharacterName)]
    public string? CharacterName { get; set; }
}