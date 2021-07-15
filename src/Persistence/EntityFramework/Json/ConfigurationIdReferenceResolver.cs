// <copyright file="ConfigurationIdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.Persistence.EntityFramework.Model;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A reference resolver which resolves them by looking at the objects occuring in the <see cref="GameConfiguration" />.
    /// The cache is maintained by instances of the <see cref="ConfigurationTypeRepository{T}" />.
    /// </summary>
    /// <remarks>TODO: I don't like it as a singleton, but I keep it until I find a cleaner solution.</remarks>
    internal class ConfigurationIdReferenceResolver : IReferenceResolver
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        private static readonly ConfigurationIdReferenceResolver InstanceValue = new ();

        private readonly IDictionary<Guid, IIdentifiable> cache = new Dictionary<Guid, IIdentifiable>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationIdReferenceResolver"/> class.
        /// </summary>
        protected ConfigurationIdReferenceResolver()
        {
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static ConfigurationIdReferenceResolver Instance => InstanceValue;

        /// <inheritdoc />
        public object ResolveReference(object context, string reference)
        {
            var id = new Guid(reference);
            if (id == Guid.Empty)
            {
                return null!;
            }

            var success = this.cache.TryGetValue(id, out var obj);
            if (!success)
            {
                return null!;
            }

            return obj!;
        }

        /// <inheritdoc/>
        public string GetReference(object context, object value)
        {
            var p = (IIdentifiable)value;

            return p.Id.ToString();
        }

        /// <inheritdoc/>
        public bool IsReferenced(object context, object value)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AddReference(object context, string reference, object value)
        {
            Guid id = new Guid(reference);
            this.cache[id] = (IIdentifiable)value;
        }

        /// <summary>
        /// Adds the reference to the cache of the resolver.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AddReference(IIdentifiable value)
        {
            this.cache[value.Id] = value;
        }

        /// <summary>
        /// Removes the reference from the cache of the resolver.
        /// </summary>
        /// <param name="key">The key.</param>
        public void RemoveReference(Guid key)
        {
            this.cache.Remove(key);
        }
    }
}
