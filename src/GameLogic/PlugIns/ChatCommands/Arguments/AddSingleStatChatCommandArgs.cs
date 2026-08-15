// <copyright file="AddSingleStatChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments of the chat commands which add points to one specific stat, e.g. <c>/addstr</c>.
/// Which stat is meant is already defined by the command itself, so the amount is the
/// only thing left to specify.
/// </summary>
/// <remarks>
/// These commands rewrite their input into the more general <c>/add</c> command and are
/// parsed with its arguments. This class just describes what a user has to enter, so it
/// must not define an <see cref="ArgumentAttribute"/> - a short name would suggest the
/// <c>amount=5</c> notation, which the arguments of <c>/add</c> don't support.
/// </remarks>
public class AddSingleStatChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the amount of points which should be added to the stat.
    /// </summary>
    public ushort Amount { get; set; }
}
