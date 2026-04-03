// <copyright file="ObjectTypeFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

/// <summary>
/// Defines the available object type filters for the map object list.
/// </summary>
internal enum ObjectTypeFilter
{
    /// <summary>All object types are shown.</summary>
    All,

    /// <summary>Only monster spawns are shown.</summary>
    Monsters,

    /// <summary>Only passive NPC spawns are shown.</summary>
    Npcs,

    /// <summary>Only enter and exit gates are shown.</summary>
    Gates,

    /// <summary>Only traps, statues, soccer balls and destructibles are shown.</summary>
    Others,
}