// <copyright file="ExtendedModelMetaData.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

// ReSharper disable UnusedMember.Global used by T4 templates
namespace MUnique.OpenMU.Persistence.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class contains information which helps to map the data model correctly for the entity framework.
    /// </summary>
    public static class ExtendedModelMetaData
    {
        private static readonly string ConfigurationNamespace = typeof(MUnique.OpenMU.DataModel.Configuration.CharacterClass).Namespace;

        /// <summary>
        /// Gets the standalone types which should not contain additional foreign key, because they were used somewhere in collections (except at <see cref="GameConfiguration"/>).
        /// For these types, join entity classes will be created and <see cref="ManyToManyCollectionAdapter{T,TJoin}"/> are used adapt between
        /// these types and the join entities.
        /// </summary>
        public static Type[] StandaloneTypes { get; } = GetStandaloneTypes().ToArray();

        /// <summary>
        /// Determines whether the given type is a is configuration type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if the given type is a configuration type; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsConfigurationType(this Type type)
        {
            return (type.Namespace?.StartsWith(ConfigurationNamespace) ?? false)
                   || (type.BaseType?.Namespace?.StartsWith(ConfigurationNamespace) ?? false)
                   || type.Name.Contains("Definition")
                   || type.Name == nameof(AttributeRelationship)
                   || type.Name == nameof(PlugInConfiguration)
                   || type.Name == nameof(ConstValueAttribute);
        }

        private static IEnumerable<Type> GetStandaloneTypes()
        {
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.CharacterClass);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.DropItemGroup);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Items.ItemDefinition);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Items.ItemOption);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionType);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Items.ItemOptionDefinition);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Items.ItemSetGroup);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.MasterSkillDefinition);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.Skill);
            yield return typeof(MUnique.OpenMU.DataModel.Configuration.GameMapDefinition);
        }
    }
}
