// <copyright file="NpcIntelligenceFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.NPC;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Creates the npc intelligences by their type name, see <see cref="DataModel.Configuration.MonsterDefinition.IntelligenceTypeName"/>.
/// </summary>
internal static partial class NpcIntelligenceFactory
{
    /// <summary>
    /// Creates the npc intelligence of the specified type, which is implemented by a code generator.
    /// </summary>
    /// <param name="typeName">The full name of the type of the intelligence.</param>
    /// <param name="map">The map, which is passed to the constructor, if it requires it.</param>
    /// <returns>The created intelligence; <c>null</c>, if the type is unknown.</returns>
    [TypeNameFactory]
    public static partial INpcIntelligence? Create(string typeName, GameMap map);
}
