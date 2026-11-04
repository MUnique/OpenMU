// <copyright file="MonsterDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Extension methods for <see cref="MonsterDefinition"/>.
/// </summary>
internal static class MonsterDefinitionExtensions
{
    /// <summary>
    /// Adds the attributes with their values to the <see cref="MonsterDefinition"/>.
    /// </summary>
    /// <param name="monsterDefinition">The monster definition.</param>
    /// <param name="attributesWithValues">The attributes with values.</param>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void AddAttributes(this MonsterDefinition monsterDefinition, IDictionary<AttributeDefinition, float> attributesWithValues, IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var attributeWithValue in attributesWithValues)
        {
            var attribute = context.CreateNew<MonsterAttribute>();
            attribute.AttributeDefinition = attributeWithValue.Key.GetPersistent(gameConfiguration);
            attribute.Value = attributeWithValue.Value;
            monsterDefinition.Attributes.Add(attribute);

            attribute.SetGuid(monsterDefinition.Number, attribute.AttributeDefinition.Id.ExtractFirstTwoBytes());
        }
    }
}