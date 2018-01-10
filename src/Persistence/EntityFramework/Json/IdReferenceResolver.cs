// <copyright file="IdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A reference resolver, which resolves based on $id references which are values of <see cref="IIdentifiable.Id"/>.
    /// </summary>
    public class IdReferenceResolver : IReferenceResolver
    {
        private readonly IDictionary<Guid, IIdentifiable> objects = new Dictionary<Guid, IIdentifiable>();

        /// <inheritdoc />
        public object ResolveReference(object context, string reference)
        {
            Guid id = new Guid(reference);

            var success = this.objects.TryGetValue(id, out IIdentifiable obj);
            if (!success)
            {
                return null;
            }

            return obj;
        }

        /// <inheritdoc/>
        public string GetReference(object context, object value)
        {
            var p = (IIdentifiable)value;
            this.objects[p.Id] = p;

            return p.Id.ToString();
        }

        /// <inheritdoc/>
        public bool IsReferenced(object context, object value)
        {
            var p = (IIdentifiable)value;
            return this.objects.ContainsKey(p.Id);
        }

        /// <inheritdoc/>
        public void AddReference(object context, string reference, object value)
        {
            Guid id = new Guid(reference);
            this.objects[id] = (IIdentifiable)value;
        }
    }
}