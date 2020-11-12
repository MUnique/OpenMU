// <copyright file="ModelGeneratorBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.SourceGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Base class for the model class generator.
    /// </summary>
    public class ModelGeneratorBase
    {
        /// <summary>
        /// The namespace of the configuration classes.
        /// </summary>
        private const string ConfigurationNamespace = "MUnique.OpenMU.DataModel.Configuration";

        private IList<Type> customTypes;

        /// <summary>
        /// Gets the types which need to be customized for persistence.
        /// </summary>
        protected IEnumerable<Type> CustomTypes => this.customTypes ??= this.GetCustomTypes();

        /// <summary>
        /// Determines whether the given type is a is configuration type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the given type is a configuration type; otherwise, <c>false</c>.</returns>
        protected static bool IsConfigurationType(Type type)
        {
            if (type.Namespace != null && type.Namespace.StartsWith(ConfigurationNamespace))
            {
                return true;
            }

            if (type.BaseType != null && type.BaseType.Namespace != null &&
                type.BaseType.Namespace.StartsWith(ConfigurationNamespace))
            {
                return true;
            }

            if (type.Name.Contains("Definition"))
            {
                return true;
            }

            if (type.Name == "AttributeRelationship" || type.Name == "PlugInConfiguration" ||
                type.Name == "ConstValueAttribute")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines the types which require customization.
        /// </summary>
        /// <returns>The types which require customization.</returns>
        private List<Type> GetCustomTypes()
        {
            var result = new List<Type>();

            var loadedTypes = typeof(DataModel.Attributes.PowerUpDefinition).Assembly.GetTypes().Where(type =>
                type.IsClass && type.IsPublic && !type.IsSealed && !type.IsAbstract &&
                type.GetConstructor(System.Array.Empty<System.Type>()) != null).ToList();
            result.AddRange(loadedTypes);
            result.Add(typeof(MUnique.OpenMU.AttributeSystem.AttributeDefinition));
            result.Add(typeof(MUnique.OpenMU.AttributeSystem.StatAttribute));
            result.Add(typeof(MUnique.OpenMU.AttributeSystem.ConstValueAttribute));
            result.Add(typeof(MUnique.OpenMU.AttributeSystem.AttributeRelationship));
            result.Add(typeof(MUnique.OpenMU.Interfaces.LetterHeader));
            result.Add(typeof(MUnique.OpenMU.Interfaces.Friend));
            result.Add(typeof(MUnique.OpenMU.PlugIns.PlugInConfiguration));

            return result;
        }
    }
}
