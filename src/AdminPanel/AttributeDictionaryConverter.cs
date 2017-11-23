// <copyright file="AttributeDictionaryConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.Persistence;
    using Nancy.Json;

    /// <summary>
    /// The attribute dictionary converter.
    /// </summary>
    public class AttributeDictionaryConverter : DefaultJavaScriptConverter
    {
        private readonly IRepositoryManager repositoryManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDictionaryConverter"/> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        public AttributeDictionaryConverter(IRepositoryManager repositoryManager)
        {
            this.repositoryManager = repositoryManager;
        }

        /// <inheritdoc/>
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(IDictionary<AttributeDefinition, float>);
            }
        }

        /// <inheritdoc/>
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            var result = new Dictionary<AttributeDefinition, float>();
            var repo = this.repositoryManager.GetRepository<AttributeDefinition>();
            foreach (var kvp in dictionary)
            {
                var attributeDefinition = repo.GetById(Guid.Parse(kvp.Key));
                result.Add(attributeDefinition, float.Parse((string)kvp.Value));
            }

            return result;
        }
    }
}
