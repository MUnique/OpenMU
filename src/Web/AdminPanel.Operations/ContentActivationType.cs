// <copyright file="ContentActivationType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The type of content which can be activated and deactivated, see <see cref="ContentActivationOperations"/>.
/// </summary>
public enum ContentActivationType
{
    /// <summary>
    /// The maps (<see cref="GameMapDefinition"/>).
    /// </summary>
    Maps,

    /// <summary>
    /// The character classes (<see cref="CharacterClass"/>).
    /// </summary>
    CharacterClasses,

    /// <summary>
    /// The monsters and NPCs (<see cref="MonsterDefinition"/>).
    /// </summary>
    Monsters,

    /// <summary>
    /// The items (<see cref="DataModel.Configuration.Items.ItemDefinition"/>).
    /// </summary>
    Items,

    /// <summary>
    /// The mini games (<see cref="MiniGameDefinition"/>).
    /// </summary>
    MiniGames,
}
