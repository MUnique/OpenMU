// <copyright file="MoveMonsterCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="MoveMonsterChatCommand"/> and others which just require an X and Y coordinate of a game map.
/// </summary>
public class MoveMonsterCommandArgs : CoordinatesCommandArgs
{
    /// <summary>
    /// Gets or sets the monster id.
    /// </summary>
    [Argument("id", true)]
    public short Id { get; set; }
}