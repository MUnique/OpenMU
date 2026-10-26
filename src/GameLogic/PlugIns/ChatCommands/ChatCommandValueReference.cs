// <copyright file="ChatCommandValueReference.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// The kind of game object a chat command parameter refers to.
/// </summary>
/// <remarks>
/// It's a hint for user interfaces, e.g. to offer a picker instead of an empty text box.
/// It's never a constraint: the parameter still accepts any value which the parser accepts.
/// The values may be sent to clients, so new members are appended at the end.
/// </remarks>
public enum ChatCommandValueReference : byte
{
    /// <summary>
    /// The parameter doesn't refer to a game object.
    /// </summary>
    None,

    /// <summary>
    /// The name of a character.
    /// </summary>
    CharacterName,

    /// <summary>
    /// The login name of an account.
    /// </summary>
    AccountName,

    /// <summary>
    /// The name of a guild.
    /// </summary>
    GuildName,

    /// <summary>
    /// A map, by its number or name.
    /// </summary>
    Map,

    /// <summary>
    /// The x-coordinate on a map.
    /// </summary>
    MapCoordinateX,

    /// <summary>
    /// The y-coordinate on a map.
    /// </summary>
    MapCoordinateY,

    /// <summary>
    /// The group of an item definition.
    /// </summary>
    ItemGroup,

    /// <summary>
    /// The number of an item definition within its group.
    /// </summary>
    ItemNumber,

    /// <summary>
    /// The number of a monster definition.
    /// </summary>
    MonsterNumber,

    /// <summary>
    /// The id of an object which is currently on the map, e.g. a monster.
    /// </summary>
    ObjectId,

    /// <summary>
    /// The number of a skill.
    /// </summary>
    SkillNumber,

    /// <summary>
    /// A language which the server supports, given by its culture name (e.g. <c>zh-CN</c>),
    /// its two-letter (e.g. <c>de</c>) or its three-letter ISO code (e.g. <c>deu</c>).
    /// </summary>
    Language,
}
