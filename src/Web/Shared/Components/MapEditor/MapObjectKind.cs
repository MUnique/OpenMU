// <copyright file="MapObjectKind.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Defines the categories of map objects which can be exported and imported.
/// </summary>
/// <remarks>
/// This is intentionally separate from <see cref="ObjectTypeFilter"/>, which is a
/// multi-select filter of the map editor and serves a different purpose.
/// </remarks>
public enum MapObjectKind
{
    /// <summary>
    /// Monster spawn areas whose <see cref="MonsterDefinition.ObjectKind"/> is a monster.
    /// </summary>
    Monsters,

    /// <summary>
    /// Spawn areas of passive NPCs and guards.
    /// </summary>
    Npcs,

    /// <summary>
    /// Spawn areas of traps, statues, soccer balls, destructibles and other objects.
    /// </summary>
    Others,

    /// <summary>
    /// Enter and exit gates. They are only exported and imported for all maps at once.
    /// </summary>
    Gates,
}
