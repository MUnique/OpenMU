// <copyright file="BanAccChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by BanAccChatCommandPlugIn.
/// </summary>
public class BanAccChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the account name.
    /// </summary>
    [Argument("acc")]
    public string? AccountName { get; set; }
}