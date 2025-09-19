// Copyright (c) MUnique. Licensed under the MIT license.

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments for the /itemstack command.
/// </summary>
public class ItemStackChatCommandArgs : ArgumentsBase
{
    [Argument("group")]
    public byte Group { get; set; }

    [Argument("number")]
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the desired amount for the stack or the number of pieces to create.
    /// </summary>
    [Argument("count")]
    public int Count { get; set; }

    /// <summary>
    /// Optional item level to set on created items (default 0).
    /// </summary>
    [Argument("lvl", false)]
    public byte Level { get; set; }
}

