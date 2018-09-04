// <copyright file="JsonObjectSerializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System.IO;
    using Newtonsoft.Json;

    /// <summary>
    /// Class to serialize an object to a json string or textwriter.
    /// </summary>
    public class JsonObjectSerializer
    {
        /// <summary>
        /// Serializes the specified object into a text writer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The text writer.</param>
        public void Serialize<T>(T obj, TextWriter textWriter)
        {
            var serializer = new JsonSerializer();
            serializer.ReferenceResolver = new IdReferenceResolver();
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            serializer.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(textWriter, obj);
        }

        /// <summary>
        /// Serializes the specified object into a string.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>The serialized object as string.</returns>
        public string Serialize<T>(T obj)
        {
            using (var textWriter = new StringWriter())
            {
                this.Serialize(obj, textWriter);
                return textWriter.ToString();
            }
        }
    }
}
