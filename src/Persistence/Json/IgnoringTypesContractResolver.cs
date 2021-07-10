// <copyright file="IgnoringTypesContractResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A contract resolver which allows to ignore properties of certain types.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.Serialization.DefaultContractResolver" />
    public class IgnoringTypesContractResolver : DefaultContractResolver
    {
        private readonly HashSet<Type> typesToIgnore;

        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoringTypesContractResolver"/> class.
        /// </summary>
        /// <param name="typesToIgnore">The types to ignore.</param>
        public IgnoringTypesContractResolver(params Type[] typesToIgnore)
        {
            this.typesToIgnore = new HashSet<Type>(typesToIgnore);
        }

        /// <inheritdoc/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var properties = base.CreateProperties(type, memberSerialization)
                .Where(p => p.PropertyType is not null && !this.typesToIgnore.Contains(p.PropertyType)).ToList();

            return properties;
        }
    }
}
