// <copyright file="ChangeLanguageChatCommandArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands.Arguments;

/// <summary>
/// Arguments used by <see cref="ChangeLanguageChatCommandArgs"/>>.
/// </summary>
public class ChangeLanguageChatCommandArgs : ArgumentsBase
{
    /// <summary>
    /// Gets or sets the iso 2/3 language code of the requested language.
    /// </summary>
    [Argument("isoCode")]
    public string? IsoLanguageCode { get; set; }
}