// <copyright file="IdReferenceResolver.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A reference resolver, which resolves based on $id references which are values of <see cref="IIdentifiable.Id" />.
    /// </summary>
    public class IdReferenceResolver : IReferenceResolver
    {
        private readonly IDictionary<Guid, IIdentifiable> objects = new Dictionary<Guid, IIdentifiable>();

        /// <inheritdoc />
        public object ResolveReference(object context, string reference)
        {
            var identity = new Guid(reference);
            var success = this.objects.TryGetValue(identity, out IIdentifiable obj);
            if (!success)
            {
                return null;
            }

            return obj;
        }

        /// <inheritdoc/>
        public string GetReference(object context, object value)
        {
            if (value is IIdentifiable identifiable)
            {
                this.objects[identifiable.Id] = identifiable;
                return identifiable.Id.ToString();
            }

            return null;
        }

        /// <inheritdoc/>
        public bool IsReferenced(object context, object value)
        {
            return value is IIdentifiable identifiable && this.objects.ContainsKey(identifiable.Id);
        }

        /// <inheritdoc/>
        public void AddReference(object context, string reference, object value)
        {
            if (value is IIdentifiable identifiable)
            {
                var identity = new Guid(reference);
                this.objects[identity] = identifiable;
            }
        }
    }
}