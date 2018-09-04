// <copyright file="JsonObjectDeserializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.Json
{
    using Newtonsoft.Json;

    /// <summary>
    /// A deserializer which parses the json retrieved from the postgres database by using a query built by the <see cref="JsonQueryBuilder"/>.
    /// We need to register a special binary converter, because postgres provides binary data in a non-standard format.
    /// </summary>
    public class JsonObjectDeserializer : MUnique.OpenMU.Persistence.Json.JsonObjectDeserializer
    {
        /// <inheritdoc/>
        protected override void BeforeDeserialize(JsonSerializer serializer)
        {
            base.BeforeDeserialize(serializer);
            serializer.Converters.Add(new BinaryAsHexJsonConverter());
        }
    }
}
