// <copyright file="AttributeDefinitionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration.Items;

    /// <summary>
    /// Extensions for <see cref="AttributeDefinition"/>.
    /// </summary>
    public static class AttributeDefinitionExtensions
    {
        /// <summary>
        /// Gets the persistent instance of <paramref name="attributeDefinition"/>.
        /// </summary>
        /// <param name="attributeDefinition">The attribue definition.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <returns>The persistent instance of <paramref name="attributeDefinition"/>.</returns>
        [Obsolete]
        public static AttributeDefinition GetPersistentOf(this AttributeDefinition attributeDefinition, IRepositoryManager repositoryManager)
        {
            return repositoryManager.GetRepository<AttributeDefinition>().GetById(attributeDefinition.Id);
        }

        /// <summary>
        /// Gets the persistent instance of <paramref name="optionType"/>.
        /// </summary>
        /// <param name="optionType">Type of the option.</param>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <returns>The persistent instance of <paramref name="optionType"/>.</returns>
        [Obsolete]
        public static ItemOptionType GetPersistentOf(this ItemOptionType optionType, IRepositoryManager repositoryManager)
        {
            return repositoryManager.GetRepository<ItemOptionType>().GetById(optionType.Id);
        }
    }
}
