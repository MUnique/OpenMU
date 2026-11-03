// <copyright file="ChatCommandValueReference.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

/// <summary>
/// Defines what kind of object the value of a chat command parameter refers to.
/// </summary>
/// <remarks>
/// It's a hint for user interfaces, so that they can offer a fitting picker
/// (e.g. a searchable list of monster names) instead of an empty input field.
/// It's never a constraint: the parsing and validation of the arguments doesn't
/// depend on it, and a user interface should always allow to enter a raw value,
/// because its data (e.g. the data files of the game client) may not match the
/// configuration of the server.
/// The numeric values may be sent to clients, so new kinds are only appended.
/// </remarks>
public enum ChatCommandValueReference
{
    /// <summary>
    /// The value doesn't refer to any known kind of object.
    /// </summary>
    None,

    /// <summary>
    /// The value is the name of a character.
    /// </summary>
    CharacterName,

    /// <summary>
    /// The value is the login name of an account.
    /// </summary>
    AccountName,

    /// <summary>
    /// The value is the name of a guild.
    /// </summary>
    GuildName,

    /// <summary>
    /// The value is the number or the name of a map.
    /// </summary>
    Map,

    /// <summary>
    /// The value is a x-coordinate on a map.
    /// </summary>
    MapCoordinateX,

    /// <summary>
    /// The value is a y-coordinate on a map.
    /// </summary>
    MapCoordinateY,

    /// <summary>
    /// The value is the group of an item definition.
    /// </summary>
    ItemGroup,

    /// <summary>
    /// The value is the number of an item definition within its group.
    /// </summary>
    ItemNumber,

    /// <summary>
    /// The value is the number of a monster definition, which also identifies its model.
    /// </summary>
    MonsterNumber,

    /// <summary>
    /// The value is the id of an object which is currently in the scope of the player.
    /// </summary>
    ObjectId,

    /// <summary>
    /// The value is the number of a skill.
    /// </summary>
    SkillNumber,

    /// <summary>
    /// The value is the ISO 639-1 code of a language, e.g. <c>en</c>.
    /// </summary>
    LanguageIsoCode,
}
