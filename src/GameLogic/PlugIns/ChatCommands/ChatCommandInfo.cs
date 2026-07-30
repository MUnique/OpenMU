// <copyright file="ChatCommandInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Describes a chat command in a machine-readable way.
/// </summary>
/// <remarks>
/// In contrast to <see cref="ChatCommandHelpAttribute.Usage"/>, which is a text
/// meant to be read by a human, this description keeps the parts of a command
/// separated. That allows a user interface to offer the commands of a player
/// without requiring the player to know or type any of them.
/// </remarks>
/// <param name="Command">The command, including the slash, e.g. <c>/item</c>.</param>
/// <param name="Name">The name of the command, in the requested language.</param>
/// <param name="Description">The description of the command, in the requested language.</param>
/// <param name="MinimumCharacterStatus">The character status which is required to execute the command.</param>
/// <param name="Usage">The usage text, as it's shown by the help command.</param>
/// <param name="Parameters">The parameters of the command, in the order in which they are expected when they are passed without their short names.</param>
public record ChatCommandInfo(
    string Command,
    string Name,
    string Description,
    CharacterStatus MinimumCharacterStatus,
    string Usage,
    IReadOnlyList<ChatCommandParameterInfo> Parameters);
