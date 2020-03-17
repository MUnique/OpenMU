// <copyright file="ObjectJsonExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Json
{
    using System.IO;

    /// <summary>
    /// Generic extension methods for the (de)serialization of objects.
    /// </summary>
    public static class ObjectJsonExtensions
    {
        /// <summary>
        /// Parses a json document string into a <typeparamref name="T" /> instance.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="jsonContent">Content of the json document.</param>
        /// <returns>
        /// The resulting <typeparamref name="T" />.
        /// </returns>
        public static T FromJson<T>(this string jsonContent)
        {
            using var stringReader = new StringReader(jsonContent);
            return stringReader.FromJson<T>();
        }

        /// <summary>
        /// Parses a json document string into a <typeparamref name="T" /> instance.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="textReader">A text reader with the content of the json document.</param>
        /// <returns>
        /// The resulting <typeparamref name="T" />.
        /// </returns>
        public static T FromJson<T>(this TextReader textReader)
        {
            var deserializer = new JsonObjectDeserializer { AreCircularReferencesExpected = true };
            var result = deserializer.Deserialize<T>(textReader, new IdReferenceResolver());
            return result;
        }

        /// <summary>
        /// Exports this instance into a json document string.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// The json document string which contains all data of this instance.
        /// </returns>
        public static string ToJson<T>(this T obj)
        {
            using var textWriter = new StringWriter();
            obj.ToJson(textWriter);
            return textWriter.ToString();
        }

        /// <summary>
        /// Exports this instance into a json document by writing into the passed text writer.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="textWriter">The target text writer, e.g. a <see cref="StringWriter" />.</param>
        public static void ToJson<T>(this T obj, TextWriter textWriter)
        {
            var serializer = new JsonObjectSerializer();
            serializer.Serialize(obj, textWriter);
        }
    }
}
