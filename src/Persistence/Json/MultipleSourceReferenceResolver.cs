// <copyright file="MultipleSourceReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A reference resolver which can resolve references by using multiple reference resolvers itself.
    /// </summary>
    public class MultipleSourceReferenceResolver : IReferenceResolver
    {
        private readonly IList<IReferenceResolver> resolvers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleSourceReferenceResolver"/> class.
        /// </summary>
        /// <param name="mainResolver">The main resolver, used to add references.</param>
        /// <param name="fallbackResolvers">The fallback resolvers.</param>
        public MultipleSourceReferenceResolver(IReferenceResolver mainResolver, params IReferenceResolver[] fallbackResolvers)
        {
            this.resolvers = new List<IReferenceResolver> { mainResolver };
            foreach (var resolver in fallbackResolvers)
            {
                this.resolvers.Add(resolver);
            }
        }

        /// <inheritdoc />
        public object ResolveReference(object context, string reference)
        {
            foreach (var resolver in this.resolvers)
            {
                var resolved = resolver.ResolveReference(context, reference);
                if (resolved != null)
                {
                    return resolved;
                }
            }

            return null;
        }

        /// <inheritdoc />
        public string GetReference(object context, object value)
        {
            return this.resolvers.First().GetReference(context, value);
        }

        /// <inheritdoc />
        public bool IsReferenced(object context, object value)
        {
            return this.resolvers.Any(resolver => resolver.IsReferenced(context, value));
        }

        /// <inheritdoc />
        public void AddReference(object context, string reference, object value)
        {
            this.resolvers.First().AddReference(context, reference, value);
        }
    }
}
