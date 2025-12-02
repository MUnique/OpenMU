// <copyright file="CreateMonsterChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="CreateMonsterChatCommand"/>.
/// </summary>
public class CreateMonsterChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the character name.
    /// </summary>
    [Argument("number")]
    public short MonsterNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the created monster should be intelligent (walking, attacking), or should do nothing at all.
    /// </summary>
    [Argument("intelligence", false)]
    public bool IsIntelligent { get; set; }
}