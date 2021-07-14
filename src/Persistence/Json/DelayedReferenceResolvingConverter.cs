// <copyright file="DelayedReferenceResolvingConverter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A json converter which is able to delay calls to the reference resolver when the resolver is unable to resolve the reference.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class DelayedReferenceResolvingConverter : JsonConverter
    {
        /// <summary>
        /// The object which is currently getting populated by the JsonSerializer.
        /// </summary>
        private object? currentlyPopulating;

        private Action? delayedReferenceResolveActions;

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override bool CanRead => true;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) =>
            !objectType.IsEnum
            && !objectType.IsGenericType
            && (objectType.Namespace?.StartsWith("MUnique.OpenMU.", StringComparison.InvariantCulture) ?? false);

        /// <summary>
        /// Resolves the delayed references.
        /// </summary>
        public void ResolveDelayedReferences() => this.delayedReferenceResolveActions?.Invoke();

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            throw new InvalidOperationException("Use default serialization.");
        }

        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jsonObject = JObject.Load(reader);

            if (jsonObject["$ref"]?.Value<string>() is { } id)
            {
                return this.ResolveObjectReference(reader, serializer, id);
            }

            return this.CreateObject(objectType, serializer, jsonObject);
        }

        private object? ResolveObjectReference(JsonReader reader, JsonSerializer serializer, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var resolvedObject = serializer.ReferenceResolver?.ResolveReference(serializer.Context, id);
            if (resolvedObject is null)
            {
                var property = this.currentlyPopulating?.GetType().GetProperty(reader.Path);
                if (property != null)
                {
                    var currentParent = this.currentlyPopulating;
                    this.delayedReferenceResolveActions += () =>
                    {
                        var resolved = serializer.ReferenceResolver?.ResolveReference(serializer.Context, id);
                        property.SetValue(currentParent, resolved);
                    };
                }
            }

            return resolvedObject;
        }

        private object CreateObject(Type objectType, JsonSerializer serializer, JObject jsonObject)
        {
            var createdObject = Activator.CreateInstance(objectType)!;

            // We need to remember the currently populating object for the next properties on the same hierarchy.
            // We could use a Stack<object> here, as well - ofc, parentPopulating lives on the "stack" too ;)
            var parentPopulating = this.currentlyPopulating;
            try
            {
                this.currentlyPopulating = createdObject;
                serializer.Populate(jsonObject.CreateReader(), createdObject);
            }
            finally
            {
                this.currentlyPopulating = parentPopulating;
            }

            return createdObject;
        }
    }
}
