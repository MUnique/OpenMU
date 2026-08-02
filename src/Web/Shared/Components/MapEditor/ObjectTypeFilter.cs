// <copyright file="ObjectTypeFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Defines the available object type filters for the map object list.
/// Multiple flags can be combined for multi-select filtering.
/// </summary>
[Flags]
public enum ObjectTypeFilter
{
    /// <summary>No filter is applied; all object types are shown.</summary>
    None = 0,

    /// <summary>Only monster spawns are shown.</summary>
    Monsters = 1,

    /// <summary>Only passive NPC spawns are shown.</summary>
    Npcs = 2,

    /// <summary>Only enter and exit gates are shown.</summary>
    Gates = 4,

    /// <summary>Only traps, statues, soccer balls and destructibles are shown.</summary>
    Others = 8,
}
