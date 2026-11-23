// <copyright file="MapObjectKindClassifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Determines the <see cref="MapObjectKind"/> of a spawn area from its monster definition.
/// </summary>
public static class MapObjectKindClassifier
{
    /// <summary>
    /// Determines the kind of the given spawn area.
    /// </summary>
    /// <param name="spawn">The spawn area.</param>
    /// <returns>The kind of the spawn area.</returns>
    public static MapObjectKind Classify(MonsterSpawnArea spawn) => Classify(spawn.MonsterDefinition);

    /// <summary>
    /// Determines the kind of a spawn area with the given monster definition.
    /// </summary>
    /// <param name="monsterDefinition">The monster definition, if any.</param>
    /// <returns>The kind of the spawn area.</returns>
    public static MapObjectKind Classify(MonsterDefinition? monsterDefinition)
    {
        return monsterDefinition?.ObjectKind switch
        {
            NpcObjectKind.Monster => MapObjectKind.Monsters,
            NpcObjectKind.PassiveNpc or NpcObjectKind.Guard => MapObjectKind.Npcs,
            _ => MapObjectKind.Others,
        };
    }
}
