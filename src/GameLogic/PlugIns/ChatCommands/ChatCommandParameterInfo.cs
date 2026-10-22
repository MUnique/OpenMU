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
/// <param name="Minimum">The smallest accepted value of a numeric parameter, if known; Otherwise, <see langword="null"/>.</param>
/// <param name="Maximum">The largest accepted value of a numeric parameter, if known; Otherwise, <see langword="null"/>.</param>
/// <param name="ValueReference">The kind of object which the value refers to. It's a hint for a user interface, never a constraint.</param>
/// <param name="ValueReferenceGroupWith">The name of the other parameter which identifies the referenced object together with this one, e.g. the group of an item; Otherwise, <see langword="null"/>.</param>
/// <remarks>
/// All of this is purely descriptive - the parsing and validation of the arguments doesn't depend on it.
/// <see cref="Minimum"/> and <see cref="Maximum"/> are values instead of a reference to an attribute, so
/// that a consumer which knows the game configuration can narrow them down where the real limit isn't
/// a compile-time constant (e.g. the maximum level of a specific item), by creating a copy with <c>with</c>.
/// </remarks>
public record ChatCommandParameterInfo(
    string Name,
    string? ShortName,
    string TypeName,
    bool IsRequired,
    IReadOnlyList<string> ValidValues,
    long? Minimum = null,
    long? Maximum = null,
    ChatCommandValueReference ValueReference = ChatCommandValueReference.None,
    string? ValueReferenceGroupWith = null);
