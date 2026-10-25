// <copyright file="ChatCommandParameterInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Describes one parameter of a chat command in a machine-readable way, so that
/// a user interface can generate an input field for it.
/// </summary>
/// <param name="Name">The name of the parameter, as defined by the property of the arguments class.</param>
/// <param name="ShortName">The short name which is used in the <c>shortName=value</c> notation, if the parameter is decorated with an <see cref="ArgumentAttribute"/>; Otherwise, <see langword="null"/>.</param>
/// <param name="TypeName">The name of the value type, e.g. <c>Byte</c> or <c>String</c>.</param>
/// <param name="IsRequired">A value indicating whether the parameter has to be specified to execute the command.</param>
/// <param name="ValidValues">The accepted values, if the parameter only accepts a limited set of them; Otherwise, empty.</param>
/// <param name="Minimum">The smallest accepted value, if the parameter has a range; Otherwise, <see langword="null"/>.</param>
/// <param name="Maximum">The biggest accepted value, if the parameter has a range; Otherwise, <see langword="null"/>.</param>
/// <param name="ValueReference">The kind of game object the parameter refers to, as a hint for a user interface.</param>
/// <param name="GroupWith">The name of another parameter which identifies the game object together with this one, e.g. the group of an item, for its number; Otherwise, <see langword="null"/>.</param>
public record ChatCommandParameterInfo(
    string Name,
    string? ShortName,
    string TypeName,
    bool IsRequired,
    IReadOnlyList<string> ValidValues,
    double? Minimum = null,
    double? Maximum = null,
    ChatCommandValueReference ValueReference = ChatCommandValueReference.None,
    string? GroupWith = null);
