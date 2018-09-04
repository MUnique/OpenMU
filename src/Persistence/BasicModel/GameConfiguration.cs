// <copyright file="GameConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.BasicModel
{
    using System.IO;
    using MUnique.OpenMU.Persistence.Json;

    /// <summary>
    /// Some additional methods for the <see cref="GameConfiguration"/>.
    /// </summary>
    public partial class GameConfiguration
    {
        /// <summary>
        /// Parses a json document string into a <see cref="GameConfiguration"/> instance.
        /// </summary>
        /// <param name="jsonContent">Content of the json document.</param>
        /// <returns>The resulting <see cref="GameConfiguration"/>.</returns>
        public static GameConfiguration FromJson(string jsonContent)
        {
            using (var stringReader = new StringReader(jsonContent))
            {
                return FromJson(stringReader);
            }
        }

        /// <summary>
        /// Parses a json document string into a <see cref="GameConfiguration"/> instance.
        /// </summary>
        /// <param name="textReader">A text reader with the content of the json document.</param>
        /// <returns>The resulting <see cref="GameConfiguration"/>.</returns>
        public static GameConfiguration FromJson(TextReader textReader)
        {
            var deserializer = new JsonObjectDeserializer { AreCircularReferencesExpected = true };
            var result = deserializer.Deserialize<GameConfiguration>(textReader, new IdReferenceResolver());
            return result;
        }

        /// <summary>
        /// Exports this instance into a json document string.
        /// </summary>
        /// <returns>The json document string which contains all data of this instance.</returns>
        public string ToJson()
        {
            using (var textWriter = new StringWriter())
            {
                this.ToJson(textWriter);
                return textWriter.ToString();
            }
        }

        /// <summary>
        /// Exports this instance into a json document by writing into the passed text writer.
        /// </summary>
        /// <param name="textWriter">The target text writer, e.g. a <see cref="StringWriter"/>.</param>
        public void ToJson(TextWriter textWriter)
        {
            var serializer = new JsonObjectSerializer();
            serializer.Serialize(this, textWriter);
        }
    }
}