// <copyright file="ItemChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by ItemChatCommandPlugIn.
/// </summary>
public class ItemChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the group.
    /// </summary>
    [Argument("group")]
    public byte Group { get; set; }

    /// <summary>
    /// Gets or sets the number.
    /// </summary>
    [Argument("number")]
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    [Argument("lvl", false)]
    public byte Level { get; set; }

    /// <summary>
    /// Gets or sets the excellent number.
    /// </summary>
    [Argument("ex", false)]
    public byte ExcellentNumber { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item contains skill.
    /// </summary>
    [Argument("sk", false)]
    public bool Skill { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item contains luck.
    /// </summary>
    [Argument("lu", false)]
    public bool Luck { get; set; }

    /// <summary>
    /// Gets or sets the option.
    /// </summary>
    [Argument("opt", false)]
    public byte Opt { get; set; }

    /// <summary>
    /// Gets or sets the ancient set discriminator.
    /// When 0, it's not an ancient.
    /// When 1, the first ancient type of an item is applied; When 2, the second, if available.
    /// Example for a Dragon Set item: 1 will be Hyon, 2 will be Vicious..
    /// </summary>
    [Argument("anc", false)]
    [ValidValues("0", "1", "2")]
    public byte Ancient { get; set; }

    /// <summary>
    /// Gets or sets the ancient bonus option; Should be 1 or 2. Only applies, when <see cref="Ancient"/> is bigger than 0.
    /// </summary>
    [Argument("ancBonuslvl", false)]
    [ValidValues("1", "2")]
    public byte AncientBonusLevel { get; set; } = 1;
}