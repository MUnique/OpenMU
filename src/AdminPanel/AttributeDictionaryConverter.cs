// <copyright file="AttributeDictionaryConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;
using MUnique.OpenMU.DataModel.Configuration;

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
        private readonly IPersistenceContextProvider persistenceContextProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeDictionaryConverter"/> class.
        /// </summary>
        /// <param name="persistenceContextProvider">The persistence context provider.</param>
        public AttributeDictionaryConverter(IPersistenceContextProvider persistenceContextProvider)
        {
            this.persistenceContextProvider = persistenceContextProvider;
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
            using (var configContext = this.persistenceContextProvider.CreateNewConfigurationContext())
            {
                var configuration = configContext.Get<GameConfiguration>().FirstOrDefault();
                using (var context = this.persistenceContextProvider.CreateNewContext(configuration))
                {
                    foreach (var kvp in dictionary)
                    {
                        var attributeDefinition = context.GetById<AttributeDefinition>(Guid.Parse(kvp.Key));
                        result.Add(attributeDefinition, float.Parse((string) kvp.Value));
                    }
                }
            }

            return result;
        }
    }
}
