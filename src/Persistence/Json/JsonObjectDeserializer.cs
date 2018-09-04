// <copyright file="JsonObjectDeserializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System.IO;
    using MUnique.OpenMU.AttributeSystem;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// A json deserializer which is able to resolve circular references.
    /// </summary>
    public class JsonObjectDeserializer
    {
        /// <summary>
        /// Gets or sets a value indicating whether circular references are expected to happen, or not.
        /// </summary>
        public bool AreCircularReferencesExpected { get; set; }

        /// <summary>
        /// Deserializes the json string to an object of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of an object to which the json string should be serialized to.</typeparam>
        /// <param name="textReader">The text reader with the json result string.</param>
        /// <param name="referenceResolver">The reference resolver.</param>
        /// <returns>
        /// The resulting object which has been deserialized from the <paramref name="textReader" />.
        /// </returns>
        public T Deserialize<T>(TextReader textReader, IReferenceResolver referenceResolver)
        {
            var serializer = new JsonSerializer();
            serializer.ReferenceResolver = referenceResolver;
            serializer.ContractResolver = new IgnoringTypesContractResolver(typeof(ConstantElement));
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;

            this.BeforeDeserialize(serializer);
            DelayedReferenceResolvingConverter deferredConverter = null;
            if (this.AreCircularReferencesExpected)
            {
                // For circular references, we add a converter which collects actions to resolve unresolved references, so they can be resolved after deserializing.
                deferredConverter = new DelayedReferenceResolvingConverter();
                serializer.Converters.Add(deferredConverter);
            }

            using (var jsonReader = new JsonTextReader(textReader))
            {
                var result = serializer.Deserialize<T>(jsonReader);
                deferredConverter?.ResolveDelayedReferences();
                return result;
            }
        }

        /// <summary>
        /// Called before the deserialization happens. Can be overwritten to apply additional settings.
        /// </summary>
        /// <param name="serializer">The serializer.</param>
        protected virtual void BeforeDeserialize(JsonSerializer serializer)
        {
            // can be overwritten to apply additional settings.
        }
    }
}
