// <copyright file="UnBanAccChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by UnBanAccChatCommandPlugIn.
/// </summary>
public class UnBanAccChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the account name.
    /// </summary>
    [Argument("acc")]
    public string? AccountName { get; set; }
}