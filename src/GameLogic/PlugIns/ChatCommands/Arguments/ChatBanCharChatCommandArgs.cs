// <copyright file="ChatBanCharChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by the <see cref="ChatBanCharChatCommandPlugIn"/>.
/// </summary>
public class ChatBanCharChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    [Argument("characterName")]
    [ValueReference(ChatCommandValueReference.CharacterName)]
    public string? CharacterName { get; set; }

    /// <summary>
    /// Gets or sets the duration.
    /// </summary>
    [Argument("durationMinutes")]
    [Range(1, int.MaxValue)]
    public int DurationMinutes { get; set; }
}